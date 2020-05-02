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
        IReadOnlyList<string> BranchBlacklist { get; set; }

        /// <summary>
        /// List of branches to always load information for.
        /// </summary>
        IReadOnlyList<string> BranchWhitelist { get; set; }

        /// <summary>
        /// Names of the <see cref="ConnectionData" /> that are used to
        /// fetch builds for this project.
        /// </summary>
        IReadOnlyList<string> BuildConnectionName { get; set; }

        /// <summary>
        /// List of build definitions to never load information for.
        /// </summary>
        IReadOnlyList<string> BuildDefinitionBlacklist { get; set; }

        /// <summary>
        /// List of build definitions to always load information for.
        /// </summary>
        IReadOnlyList<string> BuildDefinitionWhitelist { get; set; }

        /// <summary>
        /// Branch that should be used by default to use for branch comparision.
        /// </summary>
        string DefaultCompareBranch { get; set; }

        /// <summary>
        /// Whether to hide Pull-Requests that has been merged.
        /// </summary>
        bool HideCompletedPullRequests { get; set; }

        /// <summary>
        /// Whether the project is enabled and should be loaded.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Name of the project.
        /// </summary>
        string ProjectName { get; set; }

        /// <summary>
        /// Defines if and how PullRequests are displayed.
        /// </summary>
        PullRequestDisplayMode PullRequestDisplay { get; set; }

        /// <summary>
        /// Name of the <see cref="ConnectionData" /> that is used to
        /// fetch branch information for this project.
        /// </summary>
        string SourceControlConnectionName { get; set; }
    }
}