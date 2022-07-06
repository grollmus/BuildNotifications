using System;

namespace BuildNotifications.Core.Config;

/// <summary>
/// Describes configurations for which builds notifications shall be shown.
/// </summary>
[Flags]
public enum BuildNotificationModes
{
    None = 0,

    /// <summary>
    /// Builds that were manually started by the user
    /// </summary>
    RequestedByMe = 1,

    /// <summary>
    /// Builds that were automatically started (CI)
    /// </summary>
    RequestedForMe = 2,

    /// <summary>
    /// Builds that were either automatically or manually started.
    /// </summary>
    RequestedByOrForMe = RequestedByMe | RequestedForMe,

    /// <summary>
    /// For every build whether it is relevant to the user or not
    /// </summary>
    Always = 4
}