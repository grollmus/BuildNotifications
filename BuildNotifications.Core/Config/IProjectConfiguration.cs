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
        IList<string> BranchBlacklist { get; set; }

        /// <summary>
        /// List of branches to always load information for.
        /// </summary>
        IList<string> BranchWhitelist { get; set; }

        /// <summary>
        /// Names of the <see cref="ConnectionData" /> that are used to
        /// fetch builds for this project.
        /// </summary>
        IList<string> BuildConnectionNames { get; set; }

        /// <summary>
        /// List of build definitions to never load information for.
        /// </summary>
        IList<string> BuildDefinitionBlacklist { get; set; }

        /// <summary>
        /// List of build definitions to always load information for.
        /// </summary>
        IList<string> BuildDefinitionWhitelist { get; set; }

        /// <summary>
        /// Branch that should be used by default to use for branch comparision.
        /// </summary>
        string DefaultCompareBranch { get; set; }

        /// <summary>
        /// Whether to hide Pull-Requests that has been merged.
        /// </summary>
        bool HideCompletedPullRequests { get; set; }

        /// <summary>
        /// Whether only white listed branches shall be loaded.
        /// </summary>
        bool LoadWhitelistedBranchesExclusively { get; set; }

        /// <summary>
        /// Whether only white listed build definitions shall be loaded.
        /// </summary>
        bool LoadWhitelistedDefinitionsExclusively { get; set; }

        /// <summary>
        /// Name of the project.
        /// </summary>
        string ProjectName { get; set; }

        /// <summary>
        /// Whether Pull-Request "Branches" shall be displayed.
        /// </summary>
        bool ShowPullRequests { get; set; }

        /// <summary>
        /// Name of the <see cref="ConnectionData" /> that are used to
        /// fetch branch information for this project.
        /// </summary>
        IList<string> SourceControlConnectionNames { get; }
    }
}