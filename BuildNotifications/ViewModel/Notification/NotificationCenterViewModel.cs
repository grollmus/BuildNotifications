﻿using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Notification
{
    public class NotificationCenterViewModel : BaseViewModel
    {
        private readonly NotificationViewModelFactory _notificationViewModelFactory;
        private NotificationViewModel _selectedNotification;
        public RemoveTrackingObservableCollection<NotificationViewModel> Notifications { get; set; } = new RemoveTrackingObservableCollection<NotificationViewModel>();

        public NotificationCenterViewModel()
        {
            _notificationViewModelFactory = new NotificationViewModelFactory();
        }

        public NotificationViewModel SelectedNotification
        {
            get => _selectedNotification;
            set
            {
                _selectedNotification = value;
                OnPropertyChanged();
                if (_selectedNotification == null)
                    return;
                HighlightRequested?.Invoke(this, new HighlightRequestedEventArgs(_selectedNotification.BuildNodes));
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
        }

        public void ClearNotificationsOfType(NotificationType type)
        {
            var toRemove = Notifications.Where(x => x.NotificationType == type).ToList();
            foreach (var viewModel in toRemove)
            {
                Notifications.Remove(viewModel);
            }
        }
    }
}