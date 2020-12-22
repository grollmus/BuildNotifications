using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Config
{
    public class ConfigurationServiceTests
    {
        [Fact]
        public void MergeShouldAddNewConnections()
        {
            // Arrange
            var existingConfig = new Configuration();
            var newConfiguration = new Configuration();
            newConfiguration.Connections.Add(new ConnectionData
            {
                ConnectionType = ConnectionPluginType.Build,
                Name = "connection_name",
                PluginConfiguration = "plugin_configuration",
                PluginType = "plugin_type"
            });

            var serializer = Substitute.For<IConfigurationSerializer>();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();
            configurationBuilder.LoadConfiguration().Returns(existingConfig);

            var sut = new ConfigurationService(serializer, configurationBuilder);

            // Act
            sut.Merge(newConfiguration);

            // Assert
            var actual = sut.Current;

            Assert.Single(actual.Connections);
            Assert.Equal(newConfiguration.Connections[0].ConnectionType, actual.Connections[0].ConnectionType);
            Assert.Equal(newConfiguration.Connections[0].Name, actual.Connections[0].Name);
            Assert.Equal(newConfiguration.Connections[0].PluginConfiguration, actual.Connections[0].PluginConfiguration);
            Assert.Equal(newConfiguration.Connections[0].PluginType, actual.Connections[0].PluginType);
        }

        [Fact]
        public void MergeShouldAddNewProjects()
        {
            // Arrange
            var existingConfig = new Configuration();
            var newConfiguration = new Configuration();
            newConfiguration.Projects.Add(new ProjectConfiguration
            {
                ProjectName = "project_name",
                IsEnabled = true,
                PullRequestDisplay = PullRequestDisplayMode.Number,
                SourceControlConnectionName = "source",
                BuildConnectionNames = new[] {"build"},
                DefaultCompareBranch = "default",
                HideCompletedPullRequests = true,
                BranchBlacklist = new[] {"black_a"},
                BranchWhitelist = new[] {"black_b", "black_c"},
                BuildDefinitionBlacklist = new[] {"white_a", "white_b"},
                BuildDefinitionWhitelist = new[] {"white_c"}
            });

            var serializer = Substitute.For<IConfigurationSerializer>();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();
            configurationBuilder.LoadConfiguration().Returns(existingConfig);

            var sut = new ConfigurationService(serializer, configurationBuilder);

            // Act
            sut.Merge(newConfiguration);

            // Assert
            var actual = sut.Current;

            Assert.Single(actual.Projects);
            Assert.Equal(newConfiguration.Projects[0].ProjectName, actual.Projects[0].ProjectName);
            Assert.Equal(newConfiguration.Projects[0].IsEnabled, actual.Projects[0].IsEnabled);
            Assert.Equal(newConfiguration.Projects[0].PullRequestDisplay, actual.Projects[0].PullRequestDisplay);
            Assert.Equal(newConfiguration.Projects[0].SourceControlConnectionName, actual.Projects[0].SourceControlConnectionName);
            Assert.Equal(newConfiguration.Projects[0].DefaultCompareBranch, actual.Projects[0].DefaultCompareBranch);
            Assert.Equal(newConfiguration.Projects[0].BuildConnectionNames, actual.Projects[0].BuildConnectionNames);
            Assert.Equal(newConfiguration.Projects[0].HideCompletedPullRequests, actual.Projects[0].HideCompletedPullRequests);
            Assert.Equal(newConfiguration.Projects[0].BranchBlacklist, actual.Projects[0].BranchBlacklist);
            Assert.Equal(newConfiguration.Projects[0].BranchWhitelist, actual.Projects[0].BranchWhitelist);
            Assert.Equal(newConfiguration.Projects[0].BuildDefinitionBlacklist, actual.Projects[0].BuildDefinitionBlacklist);
            Assert.Equal(newConfiguration.Projects[0].BuildDefinitionWhitelist, actual.Projects[0].BuildDefinitionWhitelist);
        }

        [Fact]
        public void MergeShouldOverrideExistingConnections()
        {
            // Arrange
            var existingConfig = new Configuration();
            existingConfig.Connections.Add(new ConnectionData
            {
                ConnectionType = ConnectionPluginType.Build,
                Name = "connection_name",
                PluginConfiguration = "plugin_configuration_old",
                PluginType = "plugin_type_old"
            });
            var newConfiguration = new Configuration();
            newConfiguration.Connections.Add(new ConnectionData
            {
                ConnectionType = ConnectionPluginType.Build,
                Name = "connection_name",
                PluginConfiguration = "plugin_configuration",
                PluginType = "plugin_type"
            });

            var serializer = Substitute.For<IConfigurationSerializer>();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();
            configurationBuilder.LoadConfiguration().Returns(existingConfig);

            var sut = new ConfigurationService(serializer, configurationBuilder);

            // Act
            sut.Merge(newConfiguration);

            // Assert
            var actual = sut.Current;

            Assert.Single(actual.Connections);
            Assert.Equal(newConfiguration.Connections[0].ConnectionType, actual.Connections[0].ConnectionType);
            Assert.Equal(newConfiguration.Connections[0].Name, actual.Connections[0].Name);
            Assert.Equal(newConfiguration.Connections[0].PluginConfiguration, actual.Connections[0].PluginConfiguration);
            Assert.Equal(newConfiguration.Connections[0].PluginType, actual.Connections[0].PluginType);
        }

        [Fact]
        public void MergeShouldOverrideExistingProjects()
        {
            // Arrange
            var existingConfig = new Configuration();
            existingConfig.Projects.Add(new ProjectConfiguration
            {
                ProjectName = "project_name",
                IsEnabled = true,
                PullRequestDisplay = PullRequestDisplayMode.Number,
                SourceControlConnectionName = "source",
                BuildConnectionNames = new[] {"build"},
                DefaultCompareBranch = "default",
                HideCompletedPullRequests = true,
                BranchBlacklist = new[] {"black_a"},
                BranchWhitelist = new[] {"black_b", "black_c"},
                BuildDefinitionBlacklist = new[] {"white_a", "white_b"},
                BuildDefinitionWhitelist = new[] {"white_c"}
            });

            var newConfiguration = new Configuration();
            newConfiguration.Projects.Add(new ProjectConfiguration
            {
                ProjectName = "project_name",
                IsEnabled = false,
                PullRequestDisplay = PullRequestDisplayMode.Path,
                SourceControlConnectionName = "source_new",
                BuildConnectionNames = new[] {"build_new"},
                DefaultCompareBranch = "default_new",
                HideCompletedPullRequests = false,
                BranchBlacklist = new[] {"black_a_new"},
                BranchWhitelist = new[] {"black_b_new", "black_c_new"},
                BuildDefinitionBlacklist = new[] {"white_a_new", "white_b_new"},
                BuildDefinitionWhitelist = new[] {"white_c_new"}
            });

            var serializer = Substitute.For<IConfigurationSerializer>();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();
            configurationBuilder.LoadConfiguration().Returns(existingConfig);

            var sut = new ConfigurationService(serializer, configurationBuilder);

            // Act
            sut.Merge(newConfiguration);

            // Assert
            var actual = sut.Current;

            Assert.Single(actual.Projects);
            Assert.Equal(newConfiguration.Projects[0].ProjectName, actual.Projects[0].ProjectName);
            Assert.Equal(newConfiguration.Projects[0].IsEnabled, actual.Projects[0].IsEnabled);
            Assert.Equal(newConfiguration.Projects[0].PullRequestDisplay, actual.Projects[0].PullRequestDisplay);
            Assert.Equal(newConfiguration.Projects[0].SourceControlConnectionName, actual.Projects[0].SourceControlConnectionName);
            Assert.Equal(newConfiguration.Projects[0].DefaultCompareBranch, actual.Projects[0].DefaultCompareBranch);
            Assert.Equal(newConfiguration.Projects[0].BuildConnectionNames, actual.Projects[0].BuildConnectionNames);
            Assert.Equal(newConfiguration.Projects[0].HideCompletedPullRequests, actual.Projects[0].HideCompletedPullRequests);
            Assert.Equal(newConfiguration.Projects[0].BranchBlacklist, actual.Projects[0].BranchBlacklist);
            Assert.Equal(newConfiguration.Projects[0].BranchWhitelist, actual.Projects[0].BranchWhitelist);
            Assert.Equal(newConfiguration.Projects[0].BuildDefinitionBlacklist, actual.Projects[0].BuildDefinitionBlacklist);
            Assert.Equal(newConfiguration.Projects[0].BuildDefinitionWhitelist, actual.Projects[0].BuildDefinitionWhitelist);
        }

        [Fact]
        public void MergeShouldOverwriteCurrentValues()
        {
            // Arrange
            var existingConfig = new Configuration();
            var newConfiguration = new Configuration
            {
                AnimationSpeed = AnimationMode.Enabled,
                ApplicationTheme = Theme.Light,
                Autostart = AutostartMode.StartWithWindows,
                BuildsToShow = 123,
                CanceledBuildNotifyConfig = BuildNotificationModes.RequestedByOrForMe,
                FailedBuildNotifyConfig = BuildNotificationModes.RequestedForMe,
                Language = "es",
                ShowBusyIndicatorOnDeltaUpdates = true,
                UpdateInterval = 122,
                UsePreReleases = true,
                SucceededBuildNotifyConfig = BuildNotificationModes.RequestedForMe,
                SortingDefinition = new BuildTreeSortingDefinition(SortingDefinition.AlphabeticalDescending, SortingDefinition.DateDescending),
                GroupDefinition = new BuildTreeGroupDefinition(GroupDefinition.BuildDefinition, GroupDefinition.Source),
                PartialSucceededTreatmentMode = PartialSucceededTreatmentMode.Ignore
            };

            var serializer = Substitute.For<IConfigurationSerializer>();
            var configurationBuilder = Substitute.For<IConfigurationBuilder>();
            configurationBuilder.LoadConfiguration().Returns(existingConfig);

            var sut = new ConfigurationService(serializer, configurationBuilder);

            // Act
            sut.Merge(newConfiguration);

            // Assert
            var actual = sut.Current;

            Assert.Equal(newConfiguration.AnimationSpeed, actual.AnimationSpeed);
            Assert.Equal(newConfiguration.Autostart, actual.Autostart);
            Assert.Equal(newConfiguration.BuildsToShow, actual.BuildsToShow);
            Assert.Equal(newConfiguration.CanceledBuildNotifyConfig, actual.CanceledBuildNotifyConfig);
            Assert.Equal(newConfiguration.Connections, actual.Connections);
            Assert.Equal(newConfiguration.FailedBuildNotifyConfig, actual.FailedBuildNotifyConfig);
            Assert.Equal(newConfiguration.GroupDefinition, actual.GroupDefinition);
            Assert.Equal(newConfiguration.Language, actual.Language);
            Assert.Equal(newConfiguration.PartialSucceededTreatmentMode, actual.PartialSucceededTreatmentMode);
            Assert.Equal(newConfiguration.Projects, actual.Projects);
            Assert.Equal(newConfiguration.ShowBusyIndicatorOnDeltaUpdates, actual.ShowBusyIndicatorOnDeltaUpdates);
            Assert.Equal(newConfiguration.SortingDefinition, actual.SortingDefinition);
            Assert.Equal(newConfiguration.SucceededBuildNotifyConfig, actual.SucceededBuildNotifyConfig);
            Assert.Equal(newConfiguration.UpdateInterval, actual.UpdateInterval);
            Assert.Equal(newConfiguration.UsePreReleases, actual.UsePreReleases);
            Assert.Equal(newConfiguration.ApplicationTheme, actual.ApplicationTheme);
        }
    }
}