using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BuildNotifications.Core;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Plugin;
using BuildNotifications.ViewModel.Notification;
using BuildNotifications.ViewModel.Overlays;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings
{
    internal class TestConnectionViewModel
    {
        public TestConnectionViewModel(ConnectionData connection, IPluginRepository pluginRepository)
        {
            _connection = connection;
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

            await TestConnection(_connection);

            StatusIndicator.ClearStatus();
            Notifications.ClearNotificationsOfType(NotificationType.Progress);
        }

        private async Task TestConnection(ConnectionData connectionData)
        {
            var buildPlugin = _pluginRepository.FindBuildPlugin(connectionData.BuildPluginType);
            var sourcePlugin = _pluginRepository.FindSourceControlPlugin(connectionData.SourceControlPluginType);
            var failed = false;

            if (buildPlugin == null && sourcePlugin == null)
            {
                ReportError("ConnectionTestFailed", "NoConnectionSetup");
                failed = true;
            }

            if (buildPlugin != null && connectionData.BuildPluginConfiguration != null)
            {
                var buildResult = await buildPlugin.TestConnection(connectionData.BuildPluginConfiguration);

                if (!buildResult.IsSuccess)
                {
                    failed = true;

                    ReportError("BuildConnectionTestFailed", buildResult.ErrorMessage);
                }
            }

            if (sourcePlugin != null && connectionData.SourceControlPluginConfiguration != null)
            {
                var sourceResult = await sourcePlugin.TestConnection(connectionData.SourceControlPluginConfiguration);

                if (!sourceResult.IsSuccess)
                {
                    failed = true;

                    ReportError("SourceConnectionTestFailed", sourceResult.ErrorMessage);
                }
            }

            if (!failed)
                ReportSuccess();

            Application.Current.Dispatcher?.Invoke(() => { TestFinished?.Invoke(this, EventArgs.Empty); });
        }

        private readonly ConnectionData _connection;
        private readonly IPluginRepository _pluginRepository;
    }
}