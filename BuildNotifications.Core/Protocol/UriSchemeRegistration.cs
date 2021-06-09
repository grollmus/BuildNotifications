using System;
using System.Diagnostics;
using Microsoft.Win32;
using NLog.Fluent;

namespace BuildNotifications.Core.Protocol
{
    public static class UriSchemeRegistration
    {
        public static string StringSeparator => $"{UriScheme}:";

        public static void Register()
        {
            try
            {
                Log.Info().Message($"Registering URI scheme \"{UriScheme}\".").Write();
                using var key = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Classes\\" + UriScheme);

                var exePath = Process.GetCurrentProcess().MainModule?.FileName;

                key.SetValue("", "URL:" + FriendlyName);
                key.SetValue("URL Protocol", "");

                using (var defaultIcon = key.CreateSubKey("DefaultIcon"))
                {
                    defaultIcon.SetValue("", exePath + ",1");
                }

                using var commandKey = key.CreateSubKey(@"shell\open\command");
                commandKey.SetValue("", "\"" + exePath + "\" \"%1\"");
            }
            catch (Exception e)
            {
                Log.Error().Message("Failed to register URI protocol.").Exception(e).Write();
                throw;
            }
        }

        private const string FriendlyName = "Protocol for BuildNotifications";
        public const string UriScheme = "buildnotifications";
    }
}