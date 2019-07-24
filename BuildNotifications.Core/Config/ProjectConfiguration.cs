using System.Collections.Generic;

namespace BuildNotifications.Core.Config
{
    internal class ProjectConfiguration : IProjectConfiguration
    {
        public ProjectConfiguration()
        {
            BranchBlacklist = new List<string>();
            BranchWhitelist = new List<string>();
            BuildDefinitionBlacklist = new List<string>();
            BuildDefinitionWhitelist = new List<string>();
            BuildConnectionNames = new List<string>();
            SourceControlConnectionNames = new List<string>();

            ProjectName = string.Empty;
            DefaultCompareBranch = string.Empty;

            ShowPullRequests = true;
            HideCompletedPullRequests = true;
            LoadWhitelistedBranchesExclusively = false;
            LoadWhitelistedDefinitionsExclusively = false;
        }

        /// <inheritdoc />
        public string ProjectName { get; set; }

        /// <inheritdoc />
        public IList<string> BranchBlacklist { get; set; }

        /// <inheritdoc />
        public IList<string> BranchWhitelist { get; set; }

        /// <inheritdoc />
        public IList<string> BuildConnectionNames { get; set; }

        /// <inheritdoc />
        public IList<string> BuildDefinitionBlacklist { get; set; }

        /// <inheritdoc />
        public IList<string> BuildDefinitionWhitelist { get; set; }

        /// <inheritdoc />
        public string DefaultCompareBranch { get; set; }

        /// <inheritdoc />
        public bool HideCompletedPullRequests { get; set; }

        /// <inheritdoc />
        public bool LoadWhitelistedBranchesExclusively { get; set; }

        /// <inheritdoc />
        public bool LoadWhitelistedDefinitionsExclusively { get; set; }

        /// <inheritdoc />
        public bool ShowPullRequests { get; set; }

        /// <inheritdoc />
        public IList<string> SourceControlConnectionNames { get; set; }
    }
}