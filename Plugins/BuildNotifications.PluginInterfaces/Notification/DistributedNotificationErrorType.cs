namespace BuildNotifications.PluginInterfaces.Notification;

/// <summary>
/// Available error notification types that can be distributed
/// </summary>
public enum DistributedNotificationErrorType
{
    /// <summary>
    /// Error notification
    /// </summary>
    Error,

    /// <summary>
    /// Success notification
    /// </summary>
    Success,

    /// <summary>
    /// Build cancelled notification
    /// </summary>
    Cancel,

    /// <summary>
    /// None
    /// </summary>
    None
}