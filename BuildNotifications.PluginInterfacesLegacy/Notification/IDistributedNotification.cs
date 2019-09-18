using System;

namespace BuildNotifications.PluginInterfacesLegacy.Notification
{
    public interface IDistributedNotification
    {
        string AppIconUrl { get; }

        Guid? BasedOnNotification { get; }

        uint ColorCode { get; }
        string Content { get; }

        string ContentImageUrl { get; }

        string FeedbackArguments { get; }

        /// <summary>
        /// Describes the root of the notification. E.g. a branch failed notification, the IssueSource would be the name of the
        /// branch.
        /// </summary>
        string IssueSource { get; }

        DistributedNotificationErrorType NotificationErrorType { get; }

        DistributedNotificationType NotificationType { get; }

        string Source { get; }

        string Title { get; }
    }
}