using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces;

/// <summary>
/// Contains information about a user.
/// </summary>
[PublicAPI]
public interface IUser
{
    /// <summary>
    /// Name that is used when displaying this user.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Id of this user.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Unique name of this user.
    /// </summary>
    string UniqueName { get; }
}