using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.SourceControl
{
    /// <summary>
    /// Contains information about a PullRequest
    /// </summary>
    [PublicAPI]
    public interface IPullRequest : IBranch
    {
        /// <summary>
        /// User entered description of the PR.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Number of this PR. Should identify it uniquely in its connection.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Branch this request originates from.
        /// </summary>
        string SourceBranch { get; }

        /// <summary>
        /// Branch this request should merge into.
        /// </summary>
        string TargetBranch { get; }
    }
}