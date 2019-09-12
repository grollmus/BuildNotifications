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

        uint ColorCode { get; }

        DistributedNotificationErrorType NotificationErrorType { get; }

        DistributedNotificationType NotificationType { get; }
    }
}