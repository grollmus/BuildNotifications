using System.Collections.Generic;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.SourceControl
{
    /// <summary>
    /// Provider that can obtain branches from a source.
    /// </summary>
    [PublicAPI]
    public interface IBranchProvider
    {
        /// <summary>
        /// Amount of branches known to this provider.
        /// </summary>
        int ExistingBranchCount { get; }

        /// <summary>
        /// Extractor that can be used on branches from this provider.
        /// </summary>
        IBranchNameExtractor NameExtractor { get; }

        /// <summary>
        /// Fetches all branches and PullRequests that are available.
        /// </summary>
        /// <returns>List of all available branches and PullRequests</returns>
        IAsyncEnumerable<IBranch> FetchExistingBranches();

        /// <summary>
        /// Returns a list of branches and PullRequests that no longer exist in the source.
        /// </summary>
        /// <returns>List of removed branches and PullRequests.</returns>
        IAsyncEnumerable<IBranch> RemovedBranches();
    }
}