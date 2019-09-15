﻿using System;

namespace BuildNotifications.PluginInterfacesLegacy.Notification
{
    public interface IDistributedNotification
    {
        string Content { get; }

        string Title { get; }

        string ContentImageUrl { get; }

        string AppIconUrl { get; }

        string Source { get; }

        string FeedbackArguments { get; }

        /// <summary>
        /// Describes the root of the notification. E.g. a branch failed notification, the IssueSource would be the name of the branch.
        /// </summary>
        string IssueSource { get; }

        uint ColorCode { get; }

        Guid? BasedOnNotification { get; }

        DistributedNotificationErrorType NotificationErrorType { get; }

        DistributedNotificationType NotificationType { get; }
    }
}