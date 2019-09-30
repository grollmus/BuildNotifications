using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Anotar.NLog;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfacesLegacy.Notification;
using Application = System.Windows.Application;

namespace BuildNotifications.ViewModel.Utils
{
    internal class TrayIconHandle : INotificationProcessor
    {
        public TrayIconHandle()
        {
            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                ContextMenu = CreateContextMenu(),
                Text = "BuildNotifications"
            };

            _notifyIcon.DoubleClick += ShowWindow;
        }

        public BuildStatus BuildStatus
        {
            get => _buildStatus;
            set
            {
                _buildStatus = value;
                try
                {
                    SetIconFromBuildStatus();
                }
                catch (Exception e)
                {
                    LogTo.ErrorException("Failed to update tray icon.", e);
                }
            }
        }

        private string DefaultAppIconPath => "pack://siteoforigin:,,,/Resources/Icons/Green.ico";

        private string DefaultTrayIconPath => $"{Environment.CurrentDirectory}/Resources/Icons/icon_pending.ico".Replace('/', '\\');

        private string ErrorAppIconPath => "pack://siteoforigin:,,,/Resources/Icons/RedIntense.ico";

        private string ErrorTrayIconPath => $"{Environment.CurrentDirectory}/Resources/Icons/icon_failed.ico".Replace('/', '\\');

        public event EventHandler? ExitRequested;

        public event EventHandler? ShowWindowRequested;

        private ContextMenu CreateContextMenu()
        {
            var menu = new ContextMenu();

            menu.MenuItems.Add(StringLocalizer.ShowWindow, ShowWindow);
            menu.MenuItems.Add(StringLocalizer.Exit, Exit);

            return menu;
        }

        private void Exit(object? sender, EventArgs e)
        {
            // clear icon so it disappears from the windows tray
            _notifyIcon.Icon = null;
            LogTo.Info("Exit requested.");
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }

        private Icon GetIcon(string path)
        {
            LogTo.Info($"Updating tray icon to \"{path}\"");
            var icon = new Icon(path);
            return icon;
        }

        private ImageSource GetImageSource(string path)
        {
            LogTo.Info($"Updating app icon to \"{path}\"");
            return BitmapFrame.Create(new Uri(path));
        }

        private void SetIconFromBuildStatus()
        {
            switch (BuildStatus)
            {
                case BuildStatus.Failed:
                    _notifyIcon.Icon = GetIcon(ErrorTrayIconPath);
                    if (Application.Current?.MainWindow != null)
                        Application.Current.MainWindow.Icon = GetImageSource(ErrorAppIconPath);
                    break;
                default:
                    _notifyIcon.Icon = GetIcon(DefaultTrayIconPath);
                    if (Application.Current?.MainWindow != null)
                        Application.Current.MainWindow.Icon = GetImageSource(DefaultAppIconPath);
                    break;
            }
        }

        private void ShowWindow(object? sender, EventArgs e)
        {
            LogTo.Info("Show window requested.");
            ShowWindowRequested?.Invoke(this, EventArgs.Empty);
        }

        public void Initialize()
        {
        }

        public void Process(IDistributedNotification notification)
        {
            BuildStatus = notification.NotificationErrorType == DistributedNotificationErrorType.Error ? BuildStatus.Failed : BuildStatus.None;
        }

        public void Clear(IDistributedNotification notification)
        {
            if (BuildStatus != BuildStatus.None)
                BuildStatus = BuildStatus.None;
        }

        public void Shutdown()
        {
        }

        private readonly NotifyIcon _notifyIcon;

        private BuildStatus _buildStatus;
    }
}