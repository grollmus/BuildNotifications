using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Notification.Distribution;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Notification
{
    public class NotificationCenterViewModel : BaseViewModel
    {
        private readonly NotificationViewModelFactory _notificationViewModelFactory;
        private NotificationViewModel? _selectedNotification;
        private bool _showTimeStamp = true;
        private bool _showEmptyMessage = true;

        public RemoveTrackingObservableCollection<NotificationViewModel> Notifications { get; set; }

        public INotificationDistributor NotificationDistributor { get; } = new NotificationDistributor();

        public bool NoNotifications => ShowEmptyMessage && !Notifications.Any();

        public NotificationCenterViewModel()
        {
            _notificationViewModelFactory = new NotificationViewModelFactory();
            Notifications = new RemoveTrackingObservableCollection<NotificationViewModel>();
            Notifications.SortDescending(x => x.Timestamp);
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

        public bool ShowEmptyMessage
        {
            get => _showEmptyMessage;
            set
            {
                _showEmptyMessage = value;
                OnPropertyChanged();
            }
        }

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

        public event EventHandler<HighlightRequestedEventArgs> HighlightRequested;

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
                Notifications.Add(notification);
            }

            foreach (var notification in Notifications)
            {
                notification.InvokeTimeUntilNowUpdate();
            }

            OnPropertyChanged(nameof(NoNotifications));

            foreach (var notification in asList.Where(ShouldPublish))
            {
                NotificationDistributor.Distribute(notification);
            }
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

        public void ClearNotificationsOfType(NotificationType type)
        {
            var toRemove = Notifications.Where(x => x.NotificationType == type).ToList();
            foreach (var viewModel in toRemove)
            {
                Notifications.Remove(viewModel);
            }

            OnPropertyChanged(nameof(NoNotifications));
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
    }
}