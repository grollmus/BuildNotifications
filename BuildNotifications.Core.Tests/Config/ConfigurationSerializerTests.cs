using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.Builds;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Config
{
    public class ConfigurationSerializerTests
    {
        [Fact]
        public void LoadPredefinedConnectionsShouldNotCrashWhenFileDoesNotExist()
        {
            // Arrange
            const string fileName = "non.existing";
            var serializer = Substitute.For<ISerializer>();
            var pluginRepo = Substitute.For<IPluginRepository>();

            if (File.Exists(fileName))
                File.Delete(fileName);
            var sut = new ConfigurationSerializer(serializer, pluginRepo);

            // Act
            var actual = sut.LoadPredefinedConnections(fileName);

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual);
        }

        [Fact]
        public void LoadShouldNotCrashWhenFileDoesNotExist()
        {
            // Arrange
            const string fileName = "non.existing";
            var serializer = Substitute.For<ISerializer>();
            var pluginRepo = Substitute.For<IPluginRepository>();

            if (File.Exists(fileName))
                File.Delete(fileName);
            var sut = new ConfigurationSerializer(serializer, pluginRepo);

            // Act
            var config = sut.Load(fileName);

            // Assert
            Assert.NotNull(config);
        }

        [Fact]
        public void LoadShouldNotCreateFileWhenFileDoesNotExist()
        {
            // Arrange
            const string fileName = "non.existing";
            var serializer = Substitute.For<ISerializer>();
            var pluginRepo = Substitute.For<IPluginRepository>();

            if (File.Exists(fileName))
                File.Delete(fileName);

            var sut = new ConfigurationSerializer(serializer, pluginRepo);

            // Act
            var actual = sut.Load(fileName);

            // Assert
            Assert.NotNull(actual);
            Assert.False(File.Exists(fileName));
        }

        [Fact]
        public void LoadShouldReadSameValuesAsSaveWrote()
        {
            // Arrange
            const string fileName = nameof(LoadShouldReadSameValuesAsSaveWrote) + ".json";
            var serializer = new Serializer();
            var pluginRepo = Substitute.For<IPluginRepository>();

            var sut = new ConfigurationSerializer(serializer, pluginRepo);

            var expected = new Configuration
            {
                BuildsToShow = 2,
                CanceledBuildNotifyConfig = BuildNotificationModes.RequestedByMe,
                Connections = new List<ConnectionData>
                {
                    new ConnectionData
                    {
                        BuildPluginConfiguration = "Test123",
                        BuildPluginType = "BuildPlugin",
                        Name = "ConnectionName",
                        SourceControlPluginConfiguration = "Test234",
                        SourceControlPluginType = "SourcePlugin"
                    }
                },
                FailedBuildNotifyConfig = BuildNotificationModes.RequestedByOrForMe,
                GroupDefinition = new BuildTreeGroupDefinition(GroupDefinition.Branch, GroupDefinition.Status),
                UsePreReleases = true,
                Language = "language",
                SortingDefinition = new BuildTreeSortingDefinition(SortingDefinition.DateAscending, SortingDefinition.StatusDescending),
                UpdateInterval = 3,
                SucceededBuildNotifyConfig = BuildNotificationModes.RequestedForMe,
                Projects = new List<IProjectConfiguration>
                {
                    new ProjectConfiguration
                    {
                        ProjectName = "Project",
                        DefaultCompareBranch = "compare",
                        PullRequestDisplay = PullRequestDisplayMode.Number,
                        BuildConnectionNames = new List<string>
                        {
                            "BCN1"
                        },
                        SourceControlConnectionNames = new List<string>
                        {
                            "SCN1"
                        },
                        BranchBlacklist = new List<string>
                        {
                            "Black1"
                        },
                        BranchWhitelist = new List<string>
                        {
                            "White1"
                        },
                        BuildDefinitionBlacklist = new List<string>
                        {
                            "BlackDef1"
                        },
                        BuildDefinitionWhitelist = new List<string>
                        {
                            "WhiteDef1"
                        },
                        HideCompletedPullRequests = true
                    }
                }
            };

            // Act
            sut.Save(expected, fileName);
            var actual = sut.Load(fileName);

            // Assert
            Assert.Equal(expected.BuildsToShow, actual.BuildsToShow);
            Assert.Equal(expected.UpdateInterval, actual.UpdateInterval);
            Assert.Equal(expected.Language, actual.Language);
            Assert.Equal(expected.FailedBuildNotifyConfig, actual.FailedBuildNotifyConfig);
            Assert.Equal(expected.SucceededBuildNotifyConfig, actual.SucceededBuildNotifyConfig);
            Assert.Equal(expected.CanceledBuildNotifyConfig, actual.CanceledBuildNotifyConfig);
            Assert.Equal(expected.GroupDefinition, actual.GroupDefinition);
            Assert.Equal(expected.SortingDefinition, actual.SortingDefinition);
            Assert.Equal(expected.UsePreReleases, actual.UsePreReleases);

            Assert.Single(actual.Connections);
            Assert.Equal(expected.Connections[0].Name, actual.Connections[0].Name);
            Assert.Equal(expected.Connections[0].BuildPluginConfiguration, actual.Connections[0].BuildPluginConfiguration);
            Assert.Equal(expected.Connections[0].BuildPluginType, actual.Connections[0].BuildPluginType);
            Assert.Equal(expected.Connections[0].SourceControlPluginConfiguration, actual.Connections[0].SourceControlPluginConfiguration);
            Assert.Equal(expected.Connections[0].SourceControlPluginType, actual.Connections[0].SourceControlPluginType);

            Assert.Single(actual.Projects);
            Assert.Equal(expected.Projects[0].ProjectName, actual.Projects[0].ProjectName);
            Assert.Equal(expected.Projects[0].DefaultCompareBranch, actual.Projects[0].DefaultCompareBranch);
            Assert.Equal(expected.Projects[0].PullRequestDisplay, actual.Projects[0].PullRequestDisplay);
            Assert.Equal(expected.Projects[0].BuildConnectionNames, actual.Projects[0].BuildConnectionNames);
            Assert.Equal(expected.Projects[0].SourceControlConnectionNames, actual.Projects[0].SourceControlConnectionNames);
            Assert.Equal(expected.Projects[0].BranchBlacklist, actual.Projects[0].BranchBlacklist);
            Assert.Equal(expected.Projects[0].BuildDefinitionBlacklist, actual.Projects[0].BuildDefinitionBlacklist);
            Assert.Equal(expected.Projects[0].BranchWhitelist, actual.Projects[0].BranchWhitelist);
            Assert.Equal(expected.Projects[0].BuildDefinitionWhitelist, actual.Projects[0].BuildDefinitionWhitelist);
            Assert.Equal(expected.Projects[0].HideCompletedPullRequests, actual.Projects[0].HideCompletedPullRequests);
        }

        [Fact]
        public void LoadShouldSetBuildAndSourceControlFunctionsOfPluginRepo()
        {
            // Arrange
            const string fileName = "non.existing";
            var serializer = Substitute.For<ISerializer>();
            var pluginRepo = Substitute.For<IPluginRepository>();
            pluginRepo.Build.Returns(new List<IBuildPlugin> {Substitute.For<IBuildPlugin>()});

            if (File.Exists(fileName))
                File.Delete(fileName);

            var sut = new ConfigurationSerializer(serializer, pluginRepo);

            // Act
            var config = sut.Load(fileName);

            // Assert
            Assert.True(((Configuration) config).PossibleBuildPlugins().Any());
        }
    }
}