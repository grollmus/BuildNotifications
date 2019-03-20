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
        /// Fetches all branches that are available.
        /// </summary>
        /// <returns>List of all available branches</returns>
        IAsyncEnumerable<IBranch> FetchExistingBranches();
    }
}