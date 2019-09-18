using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BuildNotifications.Core;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.ViewModel.Notification;
using BuildNotifications.ViewModel.Overlays;
using BuildNotifications.ViewModel.Utils;
using ReflectSettings.Attributes;

namespace BuildNotifications.ViewModel.Settings
{
    public class TestConnectionViewModel
    {
        public TestConnectionViewModel(ConnectionDataViewModel connectionDataViewModel)
        {
            _connectionDataViewModel = connectionDataViewModel;
            StatusIndicator = new StatusIndicatorViewModel();
            Notifications = new NotificationCenterViewModel {ShowEmptyMessage = false, ShowTimeStamp = false};
            TestConnectionCommand = AsyncCommand.Create(TestConnection);
        }

        public bool LastTestDidSucceed { get; set; }

        [IgnoredForConfig]
        public NotificationCenterViewModel Notifications { get; set; }

        [IgnoredForConfig]
        public StatusIndicatorViewModel StatusIndicator { get; set; }

        [IgnoredForConfig]
        public ICommand TestConnectionCommand { get; set; }

        public event EventHandler TestFinished;

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
            Notifications.ShowNotifications(new List<INotification> {new StatusNotification("", "Testing", NotificationType.Info)});
            await new SynchronizationContextRemover();

            var selectedBuildData = _connectionDataViewModel.Connection;

            if (selectedBuildData == null)
                return;

            await TestConnection(selectedBuildData);
            StatusIndicator.ClearStatus();
        }

        private async Task TestConnection(ConnectionData connectionData)
        {
            var pluginRepository = _connectionDataViewModel.PluginRepository;
            if (pluginRepository == null)
                return;

            var buildPlugin = pluginRepository.FindBuildPlugin(connectionData.BuildPluginType);
            var sourcePlugin = pluginRepository.FindSourceControlPlugin(connectionData.SourceControlPluginType);
            var failed = false;

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

        private readonly ConnectionDataViewModel _connectionDataViewModel;
    }
}