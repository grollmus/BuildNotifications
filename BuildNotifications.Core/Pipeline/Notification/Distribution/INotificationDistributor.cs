using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Notification;

namespace BuildNotifications.Core.Pipeline.Notification.Distribution
{
    public interface INotificationDistributor : ICollection<INotificationProcessor>
    {
        void ClearAllMessages();
        void ClearDistributedMessage(INotification notification);
        void Distribute(INotification notification);
    }
}