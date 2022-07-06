using System;

namespace BuildNotifications.Core.Pipeline.Notification.Distribution;

/// <summary>
/// External sources may supply BN with an distributed notification. This interface describes entry
/// points for such
/// interfaces
/// </summary>
public interface IDistributedNotificationReceiver
{
    event EventHandler<DistributedNotificationReceivedEventArgs> DistributedNotificationReceived;
}