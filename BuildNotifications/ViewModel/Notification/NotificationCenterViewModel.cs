using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Notification.Distribution;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.Services;
using BuildNotifications.ViewModel.Utils;
using NLog.Fluent;

namespace BuildNotifications.ViewModel.Notification
{
    public class NotificationCenterViewModel : BaseViewModel, INotifier
    {
        public NotificationCenterViewModel()
        {
            _notificationViewModelFactory = new NotificationViewModelFactory();
            Notifications = new RemoveTrackingObservableCollection<NotificationViewModel>();
            Notifications.SortDescending(x => x.Timestamp);
            Notifications.CollectionChanged += Notifications_CollectionChanged;

            NewNotificationsCounter = new NewNotificationsCounterViewModel();
            ClearAllCommand = new DelegateCommand(x => ClearAll());
        }

        public ICommand ClearAllCommand { get; set; }

        public bool ClearButtonVisible => !Notifications.All(x => x.IsRemoving) && ShowClearButton;

        public NewNotificationsCounterViewModel NewNotificationsCounter { get; set; }

        public bool NoNotifications => ShowEmptyMessage && Notifications.All(x => x.IsRemoving);

        public INotificationDistributor NotificationDistributor { get; } = new NotificationDistributor();

        public RemoveTrackingObservableCollection<NotificationViewModel> Notifications { get; }

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

        public event EventHandler? CloseRequested;
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

        public void ClearNotificationsOfType(NotificationType type)
        {
            var toRemove = Notifications.Where(x => x.NotificationType == type).ToList();
            RemoveNotifications(toRemove);

            OnPropertyChanged(nameof(NoNotifications));
            OnPropertyChanged(nameof(ClearButtonVisible));
            UpdateUnreadCount();
        }

        public void ClearSelection()
        {
            SelectedNotification = null;
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

        private void ClearAll()
        {
            AllRead();
            ClearSelection();
            RemoveNotifications(Notifications.ToList());

            OnPropertyChanged(nameof(NoNotifications));
            OnPropertyChanged(nameof(ClearButtonVisible));
            UpdateUnreadCount();
        }

        private void Notifications_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Notifications.Count == 0)
                CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        private void RemoveNotifications(IEnumerable<NotificationViewModel> notifications)
        {
            // smoothly clear list by removing each element instead of calling .clear
            // (which would remove each item immediately not leaving any time to animate)
            foreach (var notification in notifications)
            {
                Notifications.Remove(notification);
                NotificationDistributor.ClearDistributedMessage(notification.Notification);
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

        private void UpdateUnreadCount()
        {
            var unread = Notifications.Where(x => x.IsUnread).ToList();
            NewNotificationsCounter.Count = unread.Count;

            var highestStatus = unread.Count != 0 ? unread.Max(n => n.BuildStatus) : BuildStatus.None;
            NewNotificationsCounter.HighestStatus = highestStatus;
        }

        public void ShowNotifications(IEnumerable<INotification> notifications)
        {
            var asList = notifications.ToList();
            var viewModels = _notificationViewModelFactory.Produce(asList);

            foreach (var notification in viewModels)
            {
                Log.Debug().Message($"Showing notification \"{notification.GetType().Name}\".").Write();
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

        private readonly NotificationViewModelFactory _notificationViewModelFactory;
        private NotificationViewModel? _selectedNotification;
        private bool _showClearButton;
        private bool _showEmptyMessage = true;
        private bool _showTimeStamp = true;
    }
}