using System.Linq;

namespace BuildNotifications.Core.Config;

internal class ConfigurationService : IConfigurationService
{
    public ConfigurationService(IConfigurationSerializer serializer, IConfigurationBuilder configurationBuilder)
    {
        Serializer = serializer;
        Current = configurationBuilder.LoadConfiguration();
    }

    private static bool IsSame(IProjectConfiguration x, IProjectConfiguration y) => x.ProjectName == y.ProjectName;

    private static bool IsSame(ConnectionData x, ConnectionData y) => x.ConnectionType == y.ConnectionType && x.Name == y.Name;

    private void MergeConnections(IConfiguration newConfiguration)
    {
        foreach (var newConnection in newConfiguration.Connections)
        {
            var existingConnection = Current.Connections.FirstOrDefault(x => IsSame(x, newConnection));
            if (existingConnection == null)
            {
                existingConnection = new ConnectionData
                {
                    Name = newConnection.Name,
                    ConnectionType = newConnection.ConnectionType
                };

                Current.Connections.Add(existingConnection);
            }

            existingConnection.PluginConfiguration = newConnection.PluginConfiguration;
            existingConnection.PluginType = newConnection.PluginType;
        }
    }

    private void MergeProjects(IConfiguration newConfiguration)
    {
        foreach (var newProject in newConfiguration.Projects)
        {
            var existingProject = Current.Projects.FirstOrDefault(x => IsSame(x, newProject));
            if (existingProject == null)
            {
                existingProject = new ProjectConfiguration
                {
                    ProjectName = newProject.ProjectName
                };

                Current.Projects.Add(existingProject);
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
    public IConfiguration Current { get; }

    public void Merge(IConfiguration newConfiguration)
    {
        Current.AnimationSpeed = newConfiguration.AnimationSpeed;
        Current.Autostart = newConfiguration.Autostart;
        Current.BuildsToShow = newConfiguration.BuildsToShow;
        Current.CanceledBuildNotifyConfig = newConfiguration.CanceledBuildNotifyConfig;
        Current.FailedBuildNotifyConfig = newConfiguration.FailedBuildNotifyConfig;
        Current.GroupDefinition = newConfiguration.GroupDefinition;
        Current.Language = newConfiguration.Language;
        Current.PartialSucceededTreatmentMode = newConfiguration.PartialSucceededTreatmentMode;
        Current.ShowBusyIndicatorOnDeltaUpdates = newConfiguration.ShowBusyIndicatorOnDeltaUpdates;
        Current.SortingDefinition = newConfiguration.SortingDefinition;
        Current.SucceededBuildNotifyConfig = newConfiguration.SucceededBuildNotifyConfig;
        Current.UpdateInterval = newConfiguration.UpdateInterval;
        Current.UsePreReleases = newConfiguration.UsePreReleases;
        Current.ApplicationTheme = newConfiguration.ApplicationTheme;

        MergeConnections(newConfiguration);
        MergeProjects(newConfiguration);
    }
}