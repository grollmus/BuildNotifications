using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BuildNotifications.Core.Config;
using Microsoft.Win32;
using NLog.Fluent;

namespace BuildNotifications.ViewModel.Utils.Configuration
{
    public class AutostartHelper
    {
        public AutostartHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void UpdateRegistrationForAutostart()
        {
            Log.Info().Message($"Updating autostart registration. Autostart mode: {_configuration.Autostart}").Write();

            if (InDebug())
            {
                Log.Info().Message("App was started in Debug. Not registering autostart").Write();
                return;
            }

            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetName().Name ?? "";

            if (ShouldAutostart())
            {
                Log.Info().Message("Registering Autostart.").Write();
                RegisterForAutostart(name);
            }
            else
            {
                Log.Info().Message("Deregistering Autostart.").Write();
                DeregisterForAutostart(name);
            }
        }

        private string AutostartCommand() => $"\"{AutostartLocation()}\" {StartupMode()}";

        private string AutostartLocation()
        {
            var fileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            fileName = Path.ChangeExtension(fileName, "exe");

            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? string.Empty;
            location = Directory.GetParent(location)?.FullName ?? string.Empty;

            return Path.Combine(location, fileName);
        }

        private void DeregisterForAutostart(string name)
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
                if (key == null)
                    return;

                if (key.GetValueNames().Contains(name))
                    key.DeleteValue(name);
            }
            catch (Exception e)
            {
                Log.Error().Message("Could not remove autostart from registry").Exception(e).Write();
            }
        }

        private bool InDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        private void RegisterForAutostart(string name)
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
                var cmd = AutostartCommand();

                key?.SetValue(name, cmd);
            }
            catch (Exception e)
            {
                Log.Error().Message("Could not write autostart to registry").Exception(e).Write();
            }
        }

        private bool ShouldAutostart() => _configuration.Autostart == AutostartMode.StartWithWindows || _configuration.Autostart == AutostartMode.StartWithWindowsMinimized;

        private string StartupMode() => _configuration.Autostart == AutostartMode.StartWithWindowsMinimized ? MinimizeArgument : "";

        private readonly IConfiguration _configuration;

        public const string MinimizeArgument = "--minimize";
        private const string RunKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
    }
}