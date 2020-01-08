using System;
using System.Diagnostics;
using Anotar.NLog;

namespace BuildNotifications.ViewModel.Utils
{
    internal static class Url
    {
        public static void GoTo(string? url)
        {
            if (url == null)
                return;

            LogTo.Info($"Trying to go to URL: \"{url}\"");
            if (Uri.IsWellFormedUriString(url, UriKind.Absolute))
            {
                try
                {
                    var processStartInfo = new ProcessStartInfo($"{url}")
                    {
                        UseShellExecute = true,
                        Verb = "open"
                    };

                    Process.Start(processStartInfo);
                }
                catch (Exception e)
                {
                    LogTo.WarnException($"Failed to open URL \"{url}\".", e);
                }
            }
        }
    }
}