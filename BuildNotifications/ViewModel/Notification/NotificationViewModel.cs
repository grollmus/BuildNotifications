using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Notification
{
    public class NotificationViewModel : BaseViewModel
    {
        public NotificationViewModel(INotification notification)
        {
            _notification = notification;
            Timestamp = DateTime.Now;
        }

        public IList<IBuildNode> BuildNodes => _notification.BuildNodes;

        public BuildStatus BuildStatus => _notification.Status;

        public string Content => _notification.DisplayContent;

        public IconType IconType => ResolveIconType();

        public Guid NotificationGuid => _notification.Guid;

        public NotificationType NotificationType => _notification.Type;

        public DateTime Timestamp { get; }

        public TimeSpan TimeUntilNow => Timestamp.TimespanToNow();

        public string Title => _notification.DisplayTitle;

        private bool _isUnread = true;

        public bool IsUnread
        {
            get => _isUnread;
            set
            {
                _isUnread = value;
                OnPropertyChanged();
            }
        }

        public void InvokeTimeUntilNowUpdate()
        {
            OnPropertyChanged(nameof(TimeUntilNow));
        }

        private IconType ResolveIconType()
        {
            return _notification.Type switch
            {
                NotificationType.Branch => IconType.Branch,
                NotificationType.Definition => IconType.Definition,
                NotificationType.DefinitionAndBranch => IconType.SingleBuild,
                NotificationType.Build => _notification.BuildNodes.Count == 1 ? IconType.SingleBuild : IconType.BuildNotification,
                NotificationType.Error => IconType.Lightning,
                NotificationType.Info => IconType.Info,
                NotificationType.Success => IconType.Checkmark,
                _ => IconType.None
            };
        }

        private readonly INotification _notification;
    }
}