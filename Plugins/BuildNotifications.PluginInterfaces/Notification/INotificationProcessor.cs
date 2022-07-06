using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Notification;

/// <summary>
/// Describes an object which takes notifications and processes it in any way. For example presenting
/// it to the user,
/// sending an email or storing it in a database.
/// </summary>
[PublicAPI]
public interface INotificationProcessor
{
    /// <summary>
    /// Ideally clears everything that was associated to that notification. E.g. an icon which was
    /// displayed for an error gets reset to default.
    /// </summary>
    /// <param name="notification">The notification which was cleared.</param>
    void Clear(IDistributedNotification notification);

    /// <summary>
    /// Initializes the processor. Is called before any other method.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Processes a notification. This is called whenever BuildNotifications creates a notification. E.g.
    /// builds have failed,
    /// an error has occured etc.
    /// </summary>
    /// <param name="notification">The created notification.</param>
    void Process(IDistributedNotification notification);

    /// <summary>
    /// Called whenever this instance is disposed. Used to release any resources the processor may hold.
    /// </summary>
    void Shutdown();
}