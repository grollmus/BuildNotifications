using System;
using BuildNotifications.PluginInterfaces.Notification;

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