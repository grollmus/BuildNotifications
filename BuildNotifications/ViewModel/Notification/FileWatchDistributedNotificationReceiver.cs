using System;
using System.IO;
using System.Threading.Tasks;
using BuildNotifications.Core;
using BuildNotifications.Core.Pipeline.Notification.Distribution;
using BuildNotifications.PluginInterfaces.Notification;
using NLog.Fluent;

namespace BuildNotifications.ViewModel.Notification;

internal class FileWatchDistributedNotificationReceiver : IDistributedNotificationReceiver, IDisposable
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

        Log.Info().Message($"Starting file watch on path \"{_targetDirectory}\".").Write();
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

        Log.Info().Message($"Stopping file watch on path \"{_watcher.Path}\".").Write();
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
            Log.Info().Message($"Writing distributed message to path \"{targetPath}\".").Write();
            File.WriteAllText(targetPath, base64Notification);
        }
        catch (Exception e)
        {
            Log.Error().Message("Failed to serialize and write DistributedNotification.").Exception(e).Write();
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
        Log.Info().Message($"New file in watched directory found. Path: \"{e.FullPath}\"").Write();
        Log.Debug().Message($"Waiting for {e.FullPath} to be ready.").Write();
        await WaitForFileToBeCopied(e);
        Log.Debug().Message("Waiting succeeded parsing file.").Write();
        ProcessFile(e.FullPath);
    }

    private void ProcessFile(string path)
    {
        var notification = ReadAsDistributedNotification(path);
        if (notification == null)
            return;

        Log.Debug().Message("Parsing succeeded distributing event.").Write();
        DistributedNotificationReceived?.Invoke(this, new DistributedNotificationReceivedEventArgs(notification));
    }

    private IDistributedNotification? ReadAsDistributedNotification(string path)
    {
        try
        {
            Log.Debug().Message($"Reading content and deserializing \"{path}\".").Write();
            var content = File.ReadAllText(path);
            return DistributedNotification.FromSerialized(content);
        }
        catch (Exception e)
        {
            Log.Error().Message("Failed to deserialize DistributedNotification.").Exception(e).Write();
            return null;
        }
        finally
        {
            try
            {
                Log.Info().Message($"Deleting file \"{path}\".").Write();
                File.Delete(path);
            }
            catch (Exception e)
            {
                Log.Error().Message($"Failed to cleanup Notification \"{path}\".").Exception(e).Write();
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

    public void Dispose()
    {
        _watcher?.Dispose();
    }

    public event EventHandler<DistributedNotificationReceivedEventArgs>? DistributedNotificationReceived;
    private readonly string _targetDirectory;
    private FileSystemWatcher? _watcher;

    private const string FileExtension = "distributedNotification";
}