using System;
using BuildNotifications.PluginInterfacesLegacy.Notification;

namespace BuildNotifications.Core.Pipeline.Notification.Distribution
{
    public class DistributedNotificationReceivedEventArgs : EventArgs
    {
        public IDistributedNotification DistributedNotification { get; set; }

        public DistributedNotificationReceivedEventArgs(IDistributedNotification distributedNotification)
        {
            DistributedNotification = distributedNotification;
        }
    }
}