using System.Collections.Generic;
using BuildNotifications.PluginInterfacesLegacy.Notification;

namespace BuildNotifications.Core.Pipeline.Notification
{
    public interface INotificationDistributor : ICollection<INotificationProcessor>
    {
        void Distribute(INotification notification);
    }
}