using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds
{
    /// <summary>
    /// Possible statuses of a build. Sorted by importance. Higher = more important
    /// </summary>
    [PublicAPI]
    public enum BuildStatus
    {
        /// <summary>
        /// Unknown status.
        /// </summary>
        None,

        /// <summary>
        /// Build has been cancelled.
        /// </summary>
        Cancelled,

        /// <summary>
        /// Build has not yet been finished.
        /// </summary>
        Pending,

        /// <summary>
        /// Build is currently running
        /// </summary>
        Running,

        /// <summary>
        /// Build succeeded.
        /// </summary>
        Succeeded,

        /// <summary>
        /// Build completed with warnings.
        /// </summary>
        PartiallySucceeded,

        /// <summary>
        /// Build failed with errors.
        /// </summary>
        Failed
    }
}