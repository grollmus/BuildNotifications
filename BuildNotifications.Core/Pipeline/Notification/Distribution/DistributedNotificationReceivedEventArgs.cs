using System;
using BuildNotifications.PluginInterfacesLegacy.Notification;

namespace BuildNotifications.Core.Pipeline.Notification.Distribution
{
    public class DistributedNotificationReceivedEventArgs : EventArgs
    {
        public DistributedNotificationReceivedEventArgs(IDistributedNotification distributedNotification)
        {
            DistributedNotification = distributedNotification;
        }

        public IDistributedNotification DistributedNotification { get; set; }
    }
}