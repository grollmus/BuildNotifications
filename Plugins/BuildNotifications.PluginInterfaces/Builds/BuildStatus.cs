using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds
{
    /// <summary>
    /// Possible statuses of a build.
    /// </summary>
    [PublicAPI]
    public enum BuildStatus
    {
        /// <summary>
        /// Unknown status.
        /// </summary>
        None,

        /// <summary>
        /// Build is currently running
        /// </summary>
        Running,

        /// <summary>
        /// Build has not yet been finished.
        /// </summary>
        Pending,

        /// <summary>
        /// Build failed with errors.
        /// </summary>
        Failed,

        /// <summary>
        /// Build completed with warnings.
        /// </summary>
        PartiallySucceeded,

        /// <summary>
        /// Build succeeded.
        /// </summary>
        Succeeded,

        /// <summary>
        /// Build has been cancelled.
        /// </summary>
        Cancelled
    }
}