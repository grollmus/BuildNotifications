using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Notification
{
    public sealed class NotificationViewModel : BaseViewModel, IEquatable<NotificationViewModel>
    {
        public NotificationViewModel(INotification notification)
        {
            Notification = notification;
            Timestamp = DateTime.Now;
        }

        public IList<IBuildNode> BuildNodes => Notification.BuildNodes;

        public BuildStatus BuildStatus => Notification.Status;

        public string Content => Notification.DisplayContent;

        public IconType IconType => ResolveIconType();

        public bool IsUnread
        {
            get => _isUnread;
            set
            {
                _isUnread = value;
                OnPropertyChanged();
            }
        }

        public INotification Notification { get; }

        public Guid NotificationGuid => Notification.Guid;

        public NotificationType NotificationType => Notification.Type;

        public string Source => Notification.Source;

        public DateTime Timestamp { get; }

        public TimeSpan TimeUntilNow => Timestamp.TimespanToNow();

        public string Title => Notification.DisplayTitle;

        public override bool Equals(object? obj) => Equals(obj as NotificationViewModel);

        public override int GetHashCode() => NotificationGuid.GetHashCode();

        public void InvokeTimeUntilNowUpdate()
        {
            OnPropertyChanged(nameof(TimeUntilNow));
        }

        private IconType ResolveIconType()
        {
            return Notification.Type switch
            {
                NotificationType.Branch => IconType.Branch,
                NotificationType.Definition => IconType.Definition,
                NotificationType.DefinitionAndBranch => IconType.SingleBuild,
                NotificationType.Build => Notification.BuildNodes.Count == 1 ? IconType.SingleBuild : IconType.BuildNotification,
                NotificationType.Error => IconType.Lightning,
                NotificationType.Info => IconType.Info,
                NotificationType.Success => IconType.Checkmark,
                _ => IconType.None
            };
        }

        public bool Equals(NotificationViewModel? other) => NotificationGuid == other?.NotificationGuid;

        private bool _isUnread = true;
    }
}