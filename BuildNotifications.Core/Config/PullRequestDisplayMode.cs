namespace BuildNotifications.Core.Config
{
    /// <summary>
    /// Possible modes to display a PullRequest
    /// </summary>
    public enum PullRequestDisplayMode
    {
        /// <summary>
        /// Hide PullRequests.
        /// </summary>
        None,

        /// <summary>
        /// Display id of the the PR
        /// </summary>
        Number,

        /// <summary>
        /// Display name of the PR
        /// </summary>
        Name,

        /// <summary>
        /// Display source and target branch
        /// </summary>
        Path
    }
}