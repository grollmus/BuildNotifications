using System.Collections.Generic;
using BuildNotifications.Core.Text;

namespace BuildNotifications.Core.Config
{
    public class ProjectConfiguration : IProjectConfiguration
    {
        public ProjectConfiguration()
        {
            BranchBlacklist = new List<string>();
            BranchWhitelist = new List<string>();
            BuildDefinitionBlacklist = new List<string>();
            BuildDefinitionWhitelist = new List<string>();
            BuildConnectionNames = new List<string>();
            SourceControlConnectionName = string.Empty;

            ProjectName = StringLocalizer.NewProject;
            DefaultCompareBranch = string.Empty;

            PullRequestDisplay = PullRequestDisplayMode.Number;
            HideCompletedPullRequests = true;
            IsEnabled = true;
        }

        public string SourceControlConnectionName { get; set; }

        public bool IsEnabled { get; set; }

        public string ProjectName { get; set; }

        public IReadOnlyList<string> BuildConnectionNames { get; set; }

        public IReadOnlyList<string> BranchBlacklist { get; set; }

        public IReadOnlyList<string> BranchWhitelist { get; set; }

        public IReadOnlyList<string> BuildDefinitionBlacklist { get; set; }

        public IReadOnlyList<string> BuildDefinitionWhitelist { get; set; }

        public string DefaultCompareBranch { get; set; }

        public bool HideCompletedPullRequests { get; set; }

        public PullRequestDisplayMode PullRequestDisplay { get; set; }
    }
}