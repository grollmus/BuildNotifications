using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Utilities
{
    internal interface IBranchNameExtractor
    {
        /// <summary>
        /// Extract name from branch that can be used for display in the UI.
        /// </summary>
        /// <param name="fullBranchName">Full name of the branch.</param>
        /// <param name="allBranches">List of all existing branches.</param>
        /// <returns>A name that can be used to display the branch in the UI.</returns>
        string ExtractDisplayName(string fullBranchName, IEnumerable<IBranch> allBranches);
    }
}