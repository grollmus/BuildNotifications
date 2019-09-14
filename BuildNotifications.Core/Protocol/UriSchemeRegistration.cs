using System;
using System.Diagnostics;
using Anotar.NLog;
using Microsoft.Win32;

namespace BuildNotifications.Core.Protocol
{
    public static class UriSchemeRegistration
    {
        public const string UriScheme = "buildnotifications";
        public static string StringSeparator => $"{UriScheme}:";
        private const string FriendlyName = "Protocol for BuildNotifications";

        public static void Register()
        {
            try
            {
                LogTo.Info($"Registering URI scheme \"{UriScheme}\".");
                using var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + UriScheme);
                if (key == null)
                    return;

                var exePath = Process.GetCurrentProcess().MainModule?.FileName;

                key.SetValue("", "URL:" + FriendlyName);
                key.SetValue("URL Protocol", "");

                using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                {
                    defaultIcon?.SetValue("", exePath + ",1");
                }

                using var commandKey = key.CreateSubKey(@"shell\open\command");
                commandKey?.SetValue("", "\"" + exePath + "\" \"%1\"");
            }
            catch (Exception e)
            {
                LogTo.ErrorException("Failed to register URI protocol.", e);
                throw;
            }
        }
    }
}