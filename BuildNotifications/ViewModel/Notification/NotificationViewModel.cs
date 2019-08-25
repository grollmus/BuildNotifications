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
        private readonly INotification _notification;

        public NotificationViewModel(INotification notification)
        {
            _notification = notification;
            Timestamp = DateTime.Now;
        }

        public string Title => _notification.DisplayTitle;

        public string Content => _notification.DisplayContent;

        public IconType IconType => ResolveIconType();

        private IconType ResolveIconType()
        {
            return _notification.Type switch
            {
                NotificationType.Branch => IconType.Branch,
                NotificationType.Definition => IconType.Definition,
                NotificationType.DefinitionAndBranch => IconType.SingleBuild,
                NotificationType.Build => _notification.BuildNodes.Count == 1 ? IconType.SingleBuild : IconType.BuildNotification,
                NotificationType.Error => IconType.Status,
                _ => IconType.None
            };
        }

        public NotificationType NotificationType => _notification.Type;

        public void InvokeTimeUntilNowUpdate() => OnPropertyChanged(nameof(TimeUntilNow));

        public BuildStatus BuildStatus => _notification.Status;

        public DateTime Timestamp { get; }

        public TimeSpan TimeUntilNow => Timestamp.TimespanToNow();

        public IList<IBuildNode> BuildNodes => _notification.BuildNodes;
    }
}
