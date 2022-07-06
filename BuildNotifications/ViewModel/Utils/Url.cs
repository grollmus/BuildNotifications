using System;
using System.Diagnostics;
using NLog.Fluent;

namespace BuildNotifications.ViewModel.Utils;

internal static class Url
{
    public static void GoTo(string? url)
    {
        if (url == null)
            return;

        Log.Info().Message($"Trying to go to URL: \"{url}\"").Write();
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
                Log.Warn().Message($"Failed to open URL \"{url}\".").Exception(e).Write();
            }
        }
    }
}