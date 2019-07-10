using System.Collections.Generic;

namespace BuildNotifications.Core.Config
{
    /// <summary>
    /// Data describing a project.
    /// </summary>
    public interface IProjectConfiguration
    {
        /// <summary>
        /// List of branches to never load information for.
        /// </summary>
        IList<string> BranchBlacklist { get; }

        /// <summary>
        /// List of branches to always load information for.
        /// </summary>
        IList<string> BranchWhitelist { get; }

        /// <summary>
        /// Name of the <see cref="ConnectionData" /> that is used to
        /// fetch builds for this project.
        /// </summary>
        string BuildConnectionName { get; }

        /// <summary>
        /// List of build definitions to never load information for.
        /// </summary>
        IList<string> BuildDefinitionBlacklist { get; }

        /// <summary>
        /// List of build definitions to always load information for.
        /// </summary>
        IList<string> BuildDefinitionWhitelist { get; }

        /// <summary>
        /// Branch that should be used by default to use for branch comparision.
        /// </summary>
        string DefaultCompareBranch { get; }

        /// <summary>
        /// Whether to hide Pull-Requests that has been merged.
        /// </summary>
        bool HideCompletedPullRequests { get; }

        /// <summary>
        /// Whether only white listed branches shall be loaded.
        /// </summary>
        bool LoadWhitelistedBranchesExclusively { get; set; }

        /// <summary>
        /// Whether only white listed build definitions shall be loaded.
        /// </summary>
        bool LoadWhitelistedDefinitionsExclusively { get; set; }

        /// <summary>
        /// Whether Pull-Request "Branches" shall be displayed.
        /// </summary>
        bool ShowPullRequests { get; }

        /// <summary>
        /// Name of the <see cref="ConnectionData" /> that is used to
        /// fetch branch information for this project.
        /// </summary>
        string SourceControlConnectionName { get; }
    }
}