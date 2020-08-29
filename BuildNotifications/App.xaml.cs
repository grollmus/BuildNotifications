using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using BuildNotifications.Core.Protocol;
using BuildNotifications.ViewModel.Notification;
using BuildNotifications.ViewModel.Utils.Configuration;
using NLog;
using NLog.Config;
using TweenSharp;

namespace BuildNotifications
{
    public partial class App
    {
        static App()
        {
            GlobalTweenHandler = new TweenHandler();
            _lastUpdate = TimeSpan.Zero;
        }

        public App()
        {
            SetWorkingDirectory();
            CompositionTarget.Rendering += CompositionTargetOnRendering;

            // setup global event logger
            ConfigurationItemFactory
                .Default
                .Targets
                .RegisterDefinition("GlobalErrorLog", typeof(GlobalErrorLogTarget));

            AppDomain.CurrentDomain.UnhandledException += (sender, args) => { Logger.Log(LogLevel.Fatal, args.ExceptionObject as Exception, "Global unhandled exception occurred."); };
        }

        /// <summary>
        /// Don't use LogTo. here, as the weaving would cause NLog to fail to initialize, as the line above sets up a log target
        /// </summary>
        private Logger Logger => LogManager.GetCurrentClassLogger();

        protected override void OnStartup(StartupEventArgs e)
        {
            var uriSchemeInArgs = e.Args.Any(a => a.Contains(UriSchemeRegistration.UriScheme, StringComparison.OrdinalIgnoreCase));
            GlobalDiagnosticsContext.Set("application", uriSchemeInArgs ? "invokedFromProtocol" : "default");

            if (ShouldBeMinimized(e.Args))
            {
                StartMinimized = true;
                Logger.Log(LogLevel.Info, "Minimized start requested.");
            }

            Logger.Log(LogLevel.Info, $"BuildNotifications started. Version {CurrentVersion()} Args: {string.Join(" ", e.Args)}");
            if (IsInvokedFromDistributedNotification(e))
            {
                WriteToastArgumentsToSharedMonitoredDirectory(e);

                if (OtherProcessIsRunning())
                {
                    Logger.Log(LogLevel.Info, "Shutting down application, as other instance is already running.");
                    Shutdown();
                    return;
                }

                Logger.Log(LogLevel.Info, "No other instance is running. Continuing startup");
            }
            else
                Logger.Log(LogLevel.Info, "Instance was not started from URI protocol. Initializing normally.");

            Logger.Log(LogLevel.Info, "Normal startup. Using MainWindow.xaml");
            StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            base.OnStartup(e);
        }

        private bool ShouldBeMinimized(IEnumerable<string> args) => args.Any(a => a.Contains(AutostartHelper.MinimizeArgument, StringComparison.OrdinalIgnoreCase));

        public static bool StartMinimized { get; set; }

        private void CompositionTargetOnRendering(object? sender, EventArgs e)
        {
            var renderEventArgs = e as RenderingEventArgs;
            if (renderEventArgs == null)
                return;

            if (_lastUpdate == renderEventArgs.RenderingTime)
                return;

            var delta = renderEventArgs.RenderingTime - _lastUpdate;
            _lastUpdate = renderEventArgs.RenderingTime;

            // for lag spikes, don't skip frames faster than 30fps (~33ms per frame)
            const int maxTimePerFrame = 20;
            var tooMuchTime = delta.Milliseconds - maxTimePerFrame;
            if (tooMuchTime > 0)
            {
                // ignore super big spikes
                if (tooMuchTime > 50)
                    tooMuchTime = 50;

                _lastUpdate -= TimeSpan.FromMilliseconds(tooMuchTime);
                delta = TimeSpan.FromMilliseconds(maxTimePerFrame);
            }

            GlobalTweenHandler.Update(delta.Milliseconds);
        }

        private string CurrentVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return versionInfo.FileVersion;
        }

        private bool IsInvokedFromDistributedNotification(StartupEventArgs e)
        {
            var args = string.Join("", e.Args);
            return args.Contains(UriSchemeRegistration.StringSeparator, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool OtherProcessIsRunning()
        {
            var assemblyName = Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
            // 1 is allowed, since 1 means ourselves
            return Process.GetProcessesByName(assemblyName).Length >= 2;
        }

        /// <summary>
        /// BN may be started from a different path or by URI scheme which will result in the working directory being somewhere
        /// else than the .exe itself. This methods sets the working directory to the same path as the .exe
        /// </summary>
        private void SetWorkingDirectory()
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        }

        private void WriteToastArgumentsToSharedMonitoredDirectory(StartupEventArgs e)
        {
            var args = string.Join("", e.Args);
            Logger.Log(LogLevel.Info, $"Started with protocol arguments. Arguments: {args}");
            var base64 = args.Split(UriSchemeRegistration.StringSeparator, StringSplitOptions.RemoveEmptyEntries).Last();

            FileWatchDistributedNotificationReceiver.WriteDistributedNotificationToPath(base64, new PathResolver());
        }

        private static TimeSpan _lastUpdate;
        public static readonly TweenHandler GlobalTweenHandler;
    }
}