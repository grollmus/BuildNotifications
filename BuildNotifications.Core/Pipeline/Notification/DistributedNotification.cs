using BuildNotifications.PluginInterfacesLegacy.Notification;

namespace BuildNotifications.Core.Pipeline.Notification
{
    public class DistributedNotification : IDistributedNotification
    {
        public string Content { get; set; } = "";

        public string Title { get; set; } = "";

        public string? ContentImageUrl { get; set; }

        public string? AppIconUrl { get; set; }

        public string? Source { get; set; }

        public string FeedbackArguments { get; set; } = "";

        public uint ColorCode { get; set; } = 0xffffffff;

        public DistributedNotificationErrorType NotificationErrorType { get; set; } = DistributedNotificationErrorType.None;

        public DistributedNotificationType NotificationType { get; set; } = DistributedNotificationType.General;
    }
}
