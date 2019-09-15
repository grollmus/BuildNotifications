using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Anotar.NLog;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfacesLegacy.Notification;
using Application = System.Windows.Application;

namespace BuildNotifications.ViewModel.Utils
{
    internal class TrayIconHandle : INotificationProcessor
    {
        private readonly NotifyIcon _notifyIcon;

        public event EventHandler ExitRequested;

        public event EventHandler ShowWindowRequested;

        private BuildStatus _buildStatus;

        private string DefaultTrayIconPath => $"{Environment.CurrentDirectory}/Resources/Icons/icon_pending.ico";

        private string ErrorTrayIconPath => $"{Environment.CurrentDirectory}/Resources/Icons/icon_failed.ico";

        private string ErrorAppIconPath => "pack://siteoforigin:,,,/Resources/Icons/RedIntense.ico";

        private string DefaultAppIconPath => "pack://siteoforigin:,,,/Resources/Icons/Green.ico";

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
                    LogTo.ErrorException("Failed to update tray icon.", e);
                }
            }
        }

        public TrayIconHandle()
        {
            _notifyIcon = new NotifyIcon
            {
                Visible = true,
                ContextMenu = CreateContextMenu(),
                Text = "BuildNotifications"
            };

            _notifyIcon.DoubleClick += ShowWindow;
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

        private ImageSource GetImageSource(string path) => BitmapFrame.Create(new Uri(path));

        private Icon GetIcon(string path)
        {
            LogTo.Info($"Updating tray icon to \"{path}\"");
            var icon = new Icon(path);
            return icon;
        }

        private ContextMenu CreateContextMenu()
        {
            var menu = new ContextMenu();

            menu.MenuItems.Add(StringLocalizer.Instance["ShowWindow"], ShowWindow);
            menu.MenuItems.Add(StringLocalizer.Instance["Exit"], Exit);

            return menu;
        }

        private void Exit(object? sender, EventArgs e)
        {
            // clear icon so it disappears from the windows tray
            _notifyIcon.Icon = null;
            ExitRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ShowWindow(object? sender, EventArgs e)
        {
            ShowWindowRequested?.Invoke(this, EventArgs.Empty);
        }

        public void Initialize()
        {
        }

        public void Process(IDistributedNotification notification)
        {
            BuildStatus = notification.NotificationErrorType == DistributedNotificationErrorType.Error ? BuildStatus.Failed : BuildStatus.None;
        }

        public void Shutdown()
        {
        }
    }
}