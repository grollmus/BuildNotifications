using System;
using System.IO;
using System.Reflection;
using Anotar.NLog;
using BuildNotifications.Core.Config;
using Microsoft.Win32;

namespace BuildNotifications.ViewModel.Utils
{
    public class AutostartHelper
    {
        private readonly IConfiguration _configuration;
        private const string RunKeyPath = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

        public AutostartHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string StartupMode() => _configuration.Autostart == AutostartMode.StartWithWindowsMinimized ? "--minimize" : "";

        private bool ShouldAutostart() => _configuration.Autostart == AutostartMode.StartWithWindows || _configuration.Autostart == AutostartMode.StartWithWindowsMinimized;

        private string AutostartCommand() => $"\"{AutostartLocation()}\" {StartupMode()}";

        private string AutostartLocation()
        {
            var fileName = Path.GetFileName(Assembly.GetExecutingAssembly().Location) ?? "";
            var location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            location = Directory.GetParent(location).FullName;

            return Path.Combine(location, fileName);
        }

        private bool IsAutostartEnabled(string name)
        {
            var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);
            var value = key?.GetValue(name);

            return value?.Equals(AutostartCommand()) == true;
        }

        private bool InDebug()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public void UpdateRegistrationForAutostart()
        {
            LogTo.Info($"Updating autostart registration. Autostart mode: {_configuration.Autostart}");

            if (InDebug())
            {
                LogTo.Info($"App was started in Debug. Not registering autostart");
                return;
            }

            var assembly = Assembly.GetExecutingAssembly();
            var name = assembly.GetName().Name ?? "";

            if (IsAutostartEnabled(name))
            {
                if (ShouldAutostart())
                {
                    LogTo.Info($"Autostart already registered.");
                    return;
                }

                LogTo.Info($"Deregistering Autostart.");
                DeregisterForAutostart(name);
            }
            else
            {
                if (!ShouldAutostart())
                {
                    LogTo.Info($"Autostart already not registered.");
                    return;
                }

                LogTo.Info($"Registering Autostart.");
                RegisterForAutostart(name);
            }
        }

        private void DeregisterForAutostart(string name)
        {
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true);

                key?.DeleteValue(name);
            }
            catch (Exception e)
            {
                LogTo.ErrorException("Could not write autostart to registry", e);
            }
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
                LogTo.ErrorException("Could not write autostart to registry", e);
            }
        }
    }
}