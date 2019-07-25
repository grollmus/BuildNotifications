using System.Collections.Generic;
using ReflectSettings.Attributes;

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

            ProjectName = string.Empty;
            BuildConnectionName = string.Empty;
            SourceControlConnectionName = string.Empty;
            DefaultCompareBranch = string.Empty;

            ShowPullRequests = true;
            HideCompletedPullRequests = true;
            LoadWhitelistedBranchesExclusively = false;
            LoadWhitelistedDefinitionsExclusively = false;
        }

        public string ProjectName { get; set; }

        [TypesForInstantiation(typeof(List<string>))]
        public IList<string> BranchBlacklist { get; set; }
        
        [TypesForInstantiation(typeof(List<string>))]
        public IList<string> BranchWhitelist { get; set; }

        public string BuildConnectionName { get; set; }
        
        [TypesForInstantiation(typeof(List<string>))]
        public IList<string> BuildDefinitionBlacklist { get; set; }
        
        [TypesForInstantiation(typeof(List<string>))]
        public IList<string> BuildDefinitionWhitelist { get; set; }

        public string DefaultCompareBranch { get; set; }

        public bool HideCompletedPullRequests { get; set; }

        public bool LoadWhitelistedBranchesExclusively { get; set; }

        public bool LoadWhitelistedDefinitionsExclusively { get; set; }

        public bool ShowPullRequests { get; set; }

        public string SourceControlConnectionName { get; set; }
    }
}