namespace BuildNotifications.PluginInterfaces.Builds
{
    /// <summary>
    /// Describes the reason why a build has been created.
    /// </summary>
    public enum BuildReason
    {
        /// <summary>
        /// When the build reason is not known or cannot be resolved.
        /// </summary>
        Unknown,

        /// <summary>
        /// Automatically created rolling build on code check-in. E.g. automatic for branch changes.
        /// </summary>
        CheckedIn,

        /// <summary>
        /// Scheduled during a fixed time. E.g. a nightly build.
        /// </summary>
        Scheduled,

        /// <summary>
        /// Created for a pull request.
        /// </summary>
        PullRequest,

        /// <summary>
        /// Manually created build.
        /// </summary>
        Manual,

        /// <summary>
        /// Everything else that did not fit into the other categories.
        /// </summary>
        Other
    }
}