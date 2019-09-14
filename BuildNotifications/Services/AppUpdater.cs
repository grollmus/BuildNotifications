using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Anotar.NLog;
using Newtonsoft.Json;

namespace BuildNotifications.Services
{
    internal class AppUpdater : IAppUpdater
    {
        public AppUpdater()
        {
            _updateExePath = FindUpdateExe();
            LogTo.Debug($"Update.exe should be located at {_updateExePath}");
        }

        private static string FindUpdateExe()
        {
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            var assemblyDirectory = Path.GetDirectoryName(assemblyLocation)
                                    ?? Directory.GetCurrentDirectory();

            var fullPath = Path.Combine(assemblyDirectory, "..", "update.exe");
            return fullPath;
        }

        public Task<UpdateCheckResult?> CheckForUpdates(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_updateExePath))
            {
                LogTo.Warn($"Update.exe not found. Expected it to be located at {_updateExePath}");
                return Task.FromResult<UpdateCheckResult?>(null);
            }

            return Task.Run(() =>
            {
                var pi = new ProcessStartInfo
                {
                    FileName = _updateExePath,
                    Arguments = $"--checkForUpdate={UpdateUrl}",
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                };

                var p = new Process
                {
                    StartInfo = pi
                };

                string? textResult = null;
                p.OutputDataReceived += (s, e) =>
                {
                    LogTo.Debug($"Checking: {e.Data}");
                    if (e.Data?.StartsWith("{") ?? false)
                        textResult = e.Data;
                };

                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();

                if (!string.IsNullOrWhiteSpace(textResult))
                {
                    LogTo.Info($"Updater response is: {textResult}");
                    return JsonConvert.DeserializeObject<UpdateCheckResult>(textResult);
                }

                LogTo.Warn("Got no meaningful response from updater");
                return null;
            }, cancellationToken);
        }

        public Task PerformUpdate(CancellationToken cancellationToken = default)
        {
            if (!File.Exists(_updateExePath))
            {
                LogTo.Warn($"Update.exe not found. Expected it to be located at {_updateExePath}");
                return Task.CompletedTask;
            }

            return Task.Run(() =>
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = _updateExePath,
                    RedirectStandardOutput = true,
                    Arguments = $"--update={UpdateUrl}",
                    UseShellExecute = false
                };

                var p = new Process
                {
                    StartInfo = startInfo
                };
                p.OutputDataReceived += (s, e) => { LogTo.Debug($"Updating: {e.Data}"); };

                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();
            }, cancellationToken);
        }

        private readonly string _updateExePath;
        private const string UpdateUrl = "https://github.com/grollmus/BuildNotifications";
    }
}