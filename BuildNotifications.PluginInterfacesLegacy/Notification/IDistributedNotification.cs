using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfacesLegacy.Notification
{
    [PublicAPI]
    public interface IDistributedNotification
    {
        /// <summary>
        /// Path to application icon. E.g. for a succeeded build a red BuildNotifications logo
        /// </summary>
        string AppIconUrl { get; }

        /// <summary>
        /// Display color for this notification. E.g. the red color of a failed build notification.
        /// </summary>
        uint ColorCode { get; }

        /// <summary>
        /// Text content of the notification
        /// </summary>
        string Content { get; }

        /// <summary>
        /// Path to an image which may be displayed additionally to the content.
        /// </summary>
        string ContentImageUrl { get; }

        /// <summary>
        /// Uri Protocol scheme to be used as startup arguments for BuildNotifications to provide feedback to the original message.
        /// </summary>
        string FeedbackArguments { get; }

        /// <summary>
        /// Describes the root of the notification. E.g. a branch failed notification, the IssueSource would be the name of the
        /// branch.
        /// </summary>
        string IssueSource { get; }

        /// <summary>
        /// Error type of this notification. What state is described. E.g. error for failed builds.
        /// </summary>
        DistributedNotificationErrorType NotificationErrorType { get; }

        /// <summary>
        /// Type of this notification.
        /// </summary>
        DistributedNotificationType NotificationType { get; }

        /// <summary>
        /// Source of the notification, e.g. the class for an error or the project name of builds.
        /// </summary>
        string Source { get; }

        /// <summary>
        /// Title of the notification. E.g. Title = "1 build failed" and Content = "Definition A on Branch B"
        /// </summary>
        string Title { get; }

        /// <summary>
        /// On which notification this instance is based of.
        /// </summary>
        Guid? BasedOnNotification { get; set; }
    }
}