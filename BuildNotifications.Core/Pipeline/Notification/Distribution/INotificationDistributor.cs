using System.Collections.Generic;
using BuildNotifications.PluginInterfacesLegacy.Notification;

namespace BuildNotifications.Core.Pipeline.Notification.Distribution
{
    public interface INotificationDistributor : ICollection<INotificationProcessor>
    {
        void Distribute(INotification notification);

        void ClearDistributedMessage(INotification notification);
    }
}