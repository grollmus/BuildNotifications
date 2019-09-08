using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Notification;
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

        public void ShowNotifications(IEnumerable<INotification> notifications)
        {
            var viewModels = _notificationViewModelFactory.Produce(notifications);
            foreach (var notification in viewModels)
            {
                Notifications.Add(notification);
            }

            foreach (var notification in Notifications)
            {
                notification.InvokeTimeUntilNowUpdate();
            }

            OnPropertyChanged(nameof(NoNotifications));
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
    }
}