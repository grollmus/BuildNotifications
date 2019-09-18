using System;
using System.IO;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.Core;
using BuildNotifications.Core.Pipeline.Notification.Distribution;
using BuildNotifications.PluginInterfacesLegacy.Notification;

namespace BuildNotifications.ViewModel.Notification
{
    internal class FileWatchDistributedNotificationReceiver : IDistributedNotificationReceiver
    {
        public FileWatchDistributedNotificationReceiver(IPathResolver pathResolver)
        {
            Directory.CreateDirectory(pathResolver.ConfigurationFolder);
            _targetDirectory = pathResolver.ConfigurationFolder;
        }

        public void HandleAllExistingFiles()
        {
            foreach (var file in Directory.EnumerateFiles(_targetDirectory, $"*.{FileExtension}", SearchOption.TopDirectoryOnly))
            {
                ProcessFile(file);
            }
        }

        public void Start()
        {
            if (_watcher != null)
                return;

            LogTo.Info($"Starting file watch on path \"{_targetDirectory}\".");
            _watcher = new FileSystemWatcher
            {
                Path = _targetDirectory,
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = $"*.{FileExtension}"
            };

            _watcher.Changed += OnDirectoryChanged;
            _watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            if (_watcher == null)
                return;

            LogTo.Info($"Stopping file watch on path \"{_watcher.Path}\".");
            _watcher.EnableRaisingEvents = false;
            _watcher.Changed -= OnDirectoryChanged;
            _watcher = null;
        }

        public static void WriteDistributedNotificationToPath(string base64Notification, IPathResolver pathResolver)
        {
            Directory.CreateDirectory(pathResolver.ConfigurationFolder);
            var targetPath = Path.Combine(pathResolver.ConfigurationFolder, $"{Guid.NewGuid().ToString()}.{FileExtension}");
            try
            {
                LogTo.Info($"Writing distributed message to path \"{targetPath}\".");
                File.WriteAllText(targetPath, base64Notification);
            }
            catch (Exception e)
            {
                LogTo.ErrorException("Failed to serialize and write DistributedNotification.", e);
            }
        }

        private static bool IsFileReady(string filename)
        {
            try
            {
                using var inputStream = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.None);
                return inputStream.Length > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async void OnDirectoryChanged(object sender, FileSystemEventArgs e)
        {
            LogTo.Info($"New file in watched directory found. Path: \"{e.FullPath}\"");
            LogTo.Debug($"Waiting for {e.FullPath} to be ready.");
            await WaitForFileToBeCopied(e);
            LogTo.Debug("Waiting succeeded parsing file.");
            ProcessFile(e.FullPath);
        }

        private void ProcessFile(string path)
        {
            var notification = ReadAsDistributedNotification(path);
            if (notification == null)
                return;

            LogTo.Debug("Parsing succeeded distributing event.");
            DistributedNotificationReceived?.Invoke(this, new DistributedNotificationReceivedEventArgs(notification));
        }

        private IDistributedNotification? ReadAsDistributedNotification(string path)
        {
            try
            {
                LogTo.Debug($"Reading content and deserializing \"{path}\".");
                var content = File.ReadAllText(path);
                return DistributedNotification.FromSerialized(content);
            }
            catch (Exception e)
            {
                LogTo.ErrorException("Failed to deserialize DistributedNotification.", e);
                return null;
            }
            finally
            {
                try
                {
                    LogTo.Info($"Deleting file \"{path}\".");
                    File.Delete(path);
                }
                catch (Exception e)
                {
                    LogTo.ErrorException($"Failed to cleanup Notification \"{path}\".", e);
                }
            }
        }

        private static async Task WaitForFileToBeCopied(FileSystemEventArgs e)
        {
            await Task.Run(async () =>
            {
                while (!IsFileReady(e.FullPath))
                {
                    await Task.Delay(100);
                }
            });
        }

        public event EventHandler<DistributedNotificationReceivedEventArgs> DistributedNotificationReceived;
        private readonly string _targetDirectory;
        private FileSystemWatcher? _watcher;

        private const string FileExtension = "distributedNotification";
    }
}