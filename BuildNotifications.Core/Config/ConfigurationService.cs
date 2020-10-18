using System.Linq;

namespace BuildNotifications.Core.Config
{
    internal class ConfigurationService : IConfigurationService
    {
        public ConfigurationService(IConfigurationSerializer serializer, IConfigurationBuilder configurationBuilder)
        {
            Serializer = serializer;
            CurrentConfig = configurationBuilder.LoadConfiguration();
        }

        private static bool IsSame(IProjectConfiguration x, IProjectConfiguration y) => x.ProjectName == y.ProjectName;

        private static bool IsSame(ConnectionData x, ConnectionData y) => x.ConnectionType == y.ConnectionType && x.Name == y.Name;

        private void MergeConnections(IConfiguration newConfiguration)
        {
            foreach (var newConnection in newConfiguration.Connections)
            {
                var existingConnection = CurrentConfig.Connections.FirstOrDefault(x => IsSame(x, newConnection));
                if (existingConnection == null)
                {
                    existingConnection = new ConnectionData
                    {
                        Name = newConnection.Name,
                        ConnectionType = newConnection.ConnectionType
                    };

                    CurrentConfig.Connections.Add(existingConnection);
                }

                existingConnection.PluginConfiguration = newConnection.PluginConfiguration;
                existingConnection.PluginType = newConnection.PluginType;
            }
        }

        private void MergeProjects(IConfiguration newConfiguration)
        {
            foreach (var newProject in newConfiguration.Projects)
            {
                var existingProject = CurrentConfig.Projects.FirstOrDefault(x => IsSame(x, newProject));
                if (existingProject == null)
                {
                    existingProject = new ProjectConfiguration
                    {
                        ProjectName = newProject.ProjectName
                    };

                    CurrentConfig.Projects.Add(existingProject);
                }

                existingProject.DefaultCompareBranch = newProject.DefaultCompareBranch;
                existingProject.HideCompletedPullRequests = newProject.HideCompletedPullRequests;
                existingProject.IsEnabled = newProject.IsEnabled;
                existingProject.PullRequestDisplay = newProject.PullRequestDisplay;
                existingProject.SourceControlConnectionName = newProject.SourceControlConnectionName;
                existingProject.BranchBlacklist = newProject.BranchBlacklist;
                existingProject.BranchWhitelist = newProject.BranchWhitelist;
                existingProject.BuildDefinitionWhitelist = newProject.BuildDefinitionWhitelist;
                existingProject.BuildDefinitionBlacklist = newProject.BuildDefinitionBlacklist;
                existingProject.BuildConnectionNames = newProject.BuildConnectionNames;
            }
        }

        public IConfigurationSerializer Serializer { get; }
        public IConfiguration CurrentConfig { get; }

        public void Merge(IConfiguration newConfiguration)
        {
            CurrentConfig.AnimationSpeed = newConfiguration.AnimationSpeed;
            CurrentConfig.Autostart = newConfiguration.Autostart;
            CurrentConfig.BuildsToShow = newConfiguration.BuildsToShow;
            CurrentConfig.CanceledBuildNotifyConfig = newConfiguration.CanceledBuildNotifyConfig;
            CurrentConfig.FailedBuildNotifyConfig = newConfiguration.FailedBuildNotifyConfig;
            CurrentConfig.GroupDefinition = newConfiguration.GroupDefinition;
            CurrentConfig.Language = newConfiguration.Language;
            CurrentConfig.PartialSucceededTreatmentMode = newConfiguration.PartialSucceededTreatmentMode;
            CurrentConfig.ShowBusyIndicatorOnDeltaUpdates = newConfiguration.ShowBusyIndicatorOnDeltaUpdates;
            CurrentConfig.SortingDefinition = newConfiguration.SortingDefinition;
            CurrentConfig.SucceededBuildNotifyConfig = newConfiguration.SucceededBuildNotifyConfig;
            CurrentConfig.UpdateInterval = newConfiguration.UpdateInterval;
            CurrentConfig.UsePreReleases = newConfiguration.UsePreReleases;
            CurrentConfig.ApplicationTheme = newConfiguration.ApplicationTheme;

            MergeConnections(newConfiguration);
            MergeProjects(newConfiguration);
        }
    }
}