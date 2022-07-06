using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Notification;
using NLog.Fluent;
using Application = System.Windows.Application;

namespace BuildNotifications.ViewModel.Utils;

internal class TrayIconHandle : INotificationProcessor, IDisposable
{
    public TrayIconHandle()
    {
        _notifyIcon = new NotifyIcon
        {
            Visible = true,
            ContextMenuStrip = CreateContextMenu(),
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
                Log.Error().Message("Failed to update tray icon.").Exception(e).Write();
            }
        }
    }

    private string DefaultAppIconPath => "pack://siteoforigin:,,,/Resources/Icons/Green.ico";

    private string DefaultTrayIconPath => $"{Environment.CurrentDirectory}/Resources/Icons/icon_pending.ico".Replace('/', '\\');

    private string ErrorAppIconPath => "pack://siteoforigin:,,,/Resources/Icons/RedIntense.ico";

    private string ErrorTrayIconPath => $"{Environment.CurrentDirectory}/Resources/Icons/icon_failed.ico".Replace('/', '\\');

    public event EventHandler? ExitRequested;

    public event EventHandler? ShowWindowRequested;

    private ContextMenuStrip CreateContextMenu()
    {
        var menu = new ContextMenuStrip();

        var showWindowItem = new ToolStripMenuItem(StringLocalizer.ShowWindow);
        showWindowItem.Click += ShowWindow;
        menu.Items.Add(showWindowItem);

        var exitItem = new ToolStripMenuItem(StringLocalizer.Exit);
        exitItem.Click += Exit;
        menu.Items.Add(exitItem);

        return menu;
    }

    private void Exit(object? sender, EventArgs e)
    {
        // clear icon so it disappears from the windows tray
        _notifyIcon.Icon = null;
        Log.Info().Message("Exit requested.").Write();
        ExitRequested?.Invoke(this, EventArgs.Empty);
    }

    private Icon GetIcon(string path)
    {
        Log.Info().Message($"Updating tray icon to \"{path}\"").Write();
        var icon = new Icon(path);
        return icon;
    }

    private ImageSource GetImageSource(string path)
    {
        Log.Info().Message($"Updating app icon to \"{path}\"").Write();
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
        Log.Info().Message("Show window requested.").Write();
        ShowWindowRequested?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _notifyIcon.Dispose();
    }

    public void Initialize()
    {
        // Do nothing
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
        // Do nothing
    }

    private readonly NotifyIcon _notifyIcon;

    private BuildStatus _buildStatus;
}