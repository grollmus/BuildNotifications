using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Anotar.NLog;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Notification.Distribution;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.Services;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Notification
{
    public class NotificationCenterViewModel : BaseViewModel, INotifier
    {
        public NotificationCenterViewModel()
        {
            _notificationViewModelFactory = new NotificationViewModelFactory();
            Notifications = new RemoveTrackingObservableCollection<NotificationViewModel>();
            Notifications.SortDescending(x => x.Timestamp);

            NewNotificationsCounter = new NewNotificationsCounterViewModel();
            ClearAllCommand = new DelegateCommand(x => ClearAll());
        }

        public NewNotificationsCounterViewModel NewNotificationsCounter { get; set; }

        public bool NoNotifications => ShowEmptyMessage && Notifications.All(x => x.IsRemoving);

        public INotificationDistributor NotificationDistributor { get; } = new NotificationDistributor();

        public ICommand ClearAllCommand { get; set; }

        public RemoveTrackingObservableCollection<NotificationViewModel> Notifications { get; set; }

        public NotificationViewModel? SelectedNotification
        {
            get => _selectedNotification;
            set
            {
                _selectedNotification = value;
                OnPropertyChanged();
                HighlightRequested?.Invoke(this, new HighlightRequestedEventArgs(_selectedNotification?.BuildNodes ?? new List<IBuildNode>()));
            }
        }

        public bool ClearButtonVisible => !Notifications.All(x => x.IsRemoving) && ShowClearButton;

        public bool ShowClearButton
        {
            get => _showClearButton;
            set
            {
                _showClearButton = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ClearButtonVisible));
            }
        }

        public bool ShowEmptyMessage
        {
            get => _showEmptyMessage;
            set
            {
                _showEmptyMessage = value;
                OnPropertyChanged();
            }
        }

        public bool ShowTimeStamp
        {
            get => _showTimeStamp;
            set
            {
                _showTimeStamp = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler<HighlightRequestedEventArgs>? HighlightRequested;

        public void AllRead()
        {
            var unread = Notifications.Where(x => x.IsUnread).ToList();
            foreach (var notification in unread)
            {
                notification.IsUnread = false;
            }

            UpdateUnreadCount();
        }

        private void ClearAll()
        {
            ClearSelection();
            // smoothly clear list by removing each element instead of calling .clear (which would remove each item immediately not leaving any time to animate)
            var toRemove = Notifications.ToList();
            RemoveNotifications(toRemove);

            OnPropertyChanged(nameof(NoNotifications));
            OnPropertyChanged(nameof(ClearButtonVisible));
            UpdateUnreadCount();
        }

        public void ClearNotificationsOfType(NotificationType type)
        {
            var toRemove = Notifications.Where(x => x.NotificationType == type).ToList();
            RemoveNotifications(toRemove);

            OnPropertyChanged(nameof(NoNotifications));
            OnPropertyChanged(nameof(ClearButtonVisible));
            UpdateUnreadCount();
        }

        private void RemoveNotifications(IEnumerable<NotificationViewModel> notifications)
        {
            foreach (var notification in notifications)
            {
                Notifications.Remove(notification);
                NotificationDistributor.ClearDistributedMessage(notification.Notification);
            }
        }

        public void ClearSelection()
        {
            SelectedNotification = null;
        }

        public void ShowNotifications(IEnumerable<INotification> notifications)
        {
            var asList = notifications.ToList();
            var viewModels = _notificationViewModelFactory.Produce(asList);

            foreach (var notification in viewModels)
            {
                LogTo.Debug($"Showing notification \"{notification.GetType().Name}\".");
                Notifications.Add(notification);
            }

            foreach (var notification in Notifications)
            {
                notification.InvokeTimeUntilNowUpdate();
            }

            OnPropertyChanged(nameof(NoNotifications));
            OnPropertyChanged(nameof(ClearButtonVisible));

            foreach (var notification in asList.Where(ShouldPublish))
            {
                NotificationDistributor.Distribute(notification);
            }

            UpdateUnreadCount();
        }

        private static bool ShouldPublish(INotification notification)
        {
            switch (notification.Type)
            {
                case NotificationType.Branch:
                case NotificationType.Definition:
                case NotificationType.DefinitionAndBranch:
                case NotificationType.Build:
                    return true;
                default:
                    return false;
            }
        }

        public bool TryHighlightNotificationByGuid(Guid guidOfNotification)
        {
            var notification = Notifications.FirstOrDefault(n => n.NotificationGuid == guidOfNotification);
            if (notification != null)
            {
                SelectedNotification = notification;
                return true;
            }

            return false;
        }

        private void UpdateUnreadCount()
        {
            var unread = Notifications.Where(x => x.IsUnread).ToList();
            NewNotificationsCounter.Count = unread.Count;

            var highestStatus = unread.Count != 0 ? unread.Max(n => n.BuildStatus) : BuildStatus.None;
            NewNotificationsCounter.HighestStatus = highestStatus;
        }

        private readonly NotificationViewModelFactory _notificationViewModelFactory;
        private NotificationViewModel? _selectedNotification;
        private bool _showClearButton;
        private bool _showEmptyMessage = true;
        private bool _showTimeStamp = true;
    }
}