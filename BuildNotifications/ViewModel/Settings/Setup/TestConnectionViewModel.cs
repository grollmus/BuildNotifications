using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BuildNotifications.Core;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.ViewModel.Notification;
using BuildNotifications.ViewModel.Overlays;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.Setup
{
    internal class TestConnectionViewModel
    {
        public TestConnectionViewModel(IPluginRepository pluginRepository)
        {
            _pluginRepository = pluginRepository;

            StatusIndicator = new StatusIndicatorViewModel();
            Notifications = new NotificationCenterViewModel {ShowEmptyMessage = false, ShowTimeStamp = false};
            TestConnectionCommand = AsyncCommand.Create(TestConnection);
        }

        public bool LastTestDidSucceed { get; set; }

        public NotificationCenterViewModel Notifications { get; set; }

        public StatusIndicatorViewModel StatusIndicator { get; set; }

        public ICommand TestConnectionCommand { get; set; }

        public event EventHandler? TestFinished;

        public void SetConfiguration(IPlugin plugin, IPluginConfiguration config, ConnectionPluginType connectionPluginType)
        {
            _plugin = plugin;
            _configuration = config;
            _connectionPluginType = connectionPluginType;
        }

        private ConnectionData BuildConnectionData()
        {
            var data = new ConnectionData
            {
                ConnectionType = _connectionPluginType,
                PluginConfiguration = _configuration?.Serialize(),
                PluginType = _plugin?.GetType().FullName
            };

            return data;
        }

        private void ReportError(string titleId, string message)
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                var notification = new ErrorNotification(message);
                notification.TitleTextId = titleId;
                Notifications.ShowNotifications(new List<INotification> {notification});
            });
        }

        private void ReportSuccess()
        {
            Application.Current.Dispatcher?.Invoke(() =>
            {
                var notification = new StatusNotification("ConnectionTestCaption", "ConnectionTestSuccessful", NotificationType.Success);
                Notifications.ShowNotifications(new List<INotification> {notification});
            });

            LastTestDidSucceed = true;
        }

        private async Task TestConnection()
        {
            Notifications.ClearNotificationsOfType(NotificationType.Error);
            Notifications.ClearNotificationsOfType(NotificationType.Success);
            Notifications.ClearNotificationsOfType(NotificationType.Info);
            StatusIndicator.Busy();
            Notifications.ShowNotifications(new List<INotification> {new StatusNotification("PleaseWait", "Testing", NotificationType.Progress)});
            await new SynchronizationContextRemover();

            await TestConnection(BuildConnectionData());

            StatusIndicator.ClearStatus();
            Notifications.ClearNotificationsOfType(NotificationType.Progress);
        }

        private async Task TestConnection(ConnectionData connectionData)
        {
            var failed = false;

            if (connectionData.ConnectionType == ConnectionPluginType.Build)
            {
                var buildPlugin = _pluginRepository.FindBuildPlugin(connectionData.PluginType);
                if (buildPlugin == null)
                {
                    ReportError("ConnectionTestFailed", "NoConnectionSetup");
                    failed = true;
                }

                if (buildPlugin != null && connectionData.PluginConfiguration != null)
                {
                    var buildResult = await buildPlugin.TestConnection(connectionData.PluginConfiguration);

                    if (!buildResult.IsSuccess)
                    {
                        failed = true;

                        ReportError("BuildConnectionTestFailed", buildResult.ErrorMessage);
                    }
                }
            }
            else if (connectionData.ConnectionType == ConnectionPluginType.SourceControl)
            {
                var sourcePlugin = _pluginRepository.FindSourceControlPlugin(connectionData.PluginType);

                if (sourcePlugin == null)
                {
                    ReportError("ConnectionTestFailed", "NoConnectionSetup");
                    failed = true;
                }

                if (sourcePlugin != null && connectionData.PluginConfiguration != null)
                {
                    var sourceResult = await sourcePlugin.TestConnection(connectionData.PluginConfiguration);

                    if (!sourceResult.IsSuccess)
                    {
                        failed = true;

                        ReportError("SourceConnectionTestFailed", sourceResult.ErrorMessage);
                    }
                }
            }

            if (!failed)
                ReportSuccess();

            Application.Current.Dispatcher?.Invoke(() => { TestFinished?.Invoke(this, EventArgs.Empty); });
        }

        private readonly IPluginRepository _pluginRepository;
        private IPluginConfiguration? _configuration;
        private ConnectionPluginType _connectionPluginType;
        private IPlugin? _plugin;
    }
}