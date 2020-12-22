using System.Collections.Generic;
using System.IO;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Core.Utilities;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Config
{
    public class ConfigurationSerializerTests
    {
        [Fact]
        public void LoadShouldLogWhenFileCantBeWritten()
        {
            // Arrange
            var serializer = new Serializer();
            var sut = new ConfigurationSerializer(serializer);

            var fileName = Path.GetRandomFileName();

            // Act
            try
            {
                using var lockStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

                using var logger = new TestLogger();

                sut.Load(fileName, out _);

                // Assert
                Assert.Contains(logger.Messages, m => m.Contains("Failed to load existing config"));
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        public void LoadShouldNotCrashWhenFileDoesNotExist()
        {
            // Arrange
            const string fileName = "non.existing";
            var serializer = Substitute.For<ISerializer>();

            if (File.Exists(fileName))
                File.Delete(fileName);
            var sut = new ConfigurationSerializer(serializer);

            // Act
            var config = sut.Load(fileName, out _);

            // Assert
            Assert.NotNull(config);
        }

        [Fact]
        public void LoadShouldNotCreateFileWhenFileDoesNotExist()
        {
            // Arrange
            const string fileName = "non.existing";
            var serializer = Substitute.For<ISerializer>();

            if (File.Exists(fileName))
                File.Delete(fileName);

            var sut = new ConfigurationSerializer(serializer);

            // Act
            var actual = sut.Load(fileName, out _);

            // Assert
            Assert.NotNull(actual);
            Assert.False(File.Exists(fileName));
        }

        [Fact]
        public void LoadShouldNotThrowWhenFileCantBeWritten()
        {
            // Arrange
            var serializer = new Serializer();
            var sut = new ConfigurationSerializer(serializer);

            var fileName = Path.GetRandomFileName();

            try
            {
                using var lockStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

                // Act
                var ex = Record.Exception(() => sut.Load(fileName, out _));

                // Assert
                Assert.Null(ex);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Fact]
        public void LoadShouldReadSameValuesAsSaveWrote()
        {
            // Arrange
            const string fileName = nameof(LoadShouldReadSameValuesAsSaveWrote) + ".json";
            var serializer = new Serializer();

            var sut = new ConfigurationSerializer(serializer);

            var expected = new Configuration
            {
                BuildsToShow = 2,
                CanceledBuildNotifyConfig = BuildNotificationModes.RequestedByMe,
                Connections = new List<ConnectionData>
                {
                    new ConnectionData
                    {
                        PluginConfiguration = "Test123",
                        PluginType = "BuildPlugin",
                        Name = "ConnectionName"
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
                        SourceControlConnectionName = "SCN1",
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
            var actual = sut.Load(fileName, out _);

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
            Assert.Equal(expected.Connections[0].PluginConfiguration, actual.Connections[0].PluginConfiguration);
            Assert.Equal(expected.Connections[0].PluginType, actual.Connections[0].PluginType);

            Assert.Single(actual.Projects);
            Assert.Equal(expected.Projects[0].ProjectName, actual.Projects[0].ProjectName);
            Assert.Equal(expected.Projects[0].DefaultCompareBranch, actual.Projects[0].DefaultCompareBranch);
            Assert.Equal(expected.Projects[0].PullRequestDisplay, actual.Projects[0].PullRequestDisplay);
            Assert.Equal(expected.Projects[0].BuildConnectionNames, actual.Projects[0].BuildConnectionNames);
            Assert.Equal(expected.Projects[0].SourceControlConnectionName, actual.Projects[0].SourceControlConnectionName);
            Assert.Equal(expected.Projects[0].BranchBlacklist, actual.Projects[0].BranchBlacklist);
            Assert.Equal(expected.Projects[0].BuildDefinitionBlacklist, actual.Projects[0].BuildDefinitionBlacklist);
            Assert.Equal(expected.Projects[0].BranchWhitelist, actual.Projects[0].BranchWhitelist);
            Assert.Equal(expected.Projects[0].BuildDefinitionWhitelist, actual.Projects[0].BuildDefinitionWhitelist);
            Assert.Equal(expected.Projects[0].HideCompletedPullRequests, actual.Projects[0].HideCompletedPullRequests);
        }

        [Fact]
        public void SaveShouldCreateDirectoryWhenItDoesNotExist()
        {
            // Arrange
            var serializer = new Serializer();
            var sut = new ConfigurationSerializer(serializer);
            var folderName = Path.GetRandomFileName();
            var fileName = Path.Combine(folderName, Path.GetRandomFileName());

            var configuration = new Configuration();

            try
            {
                // Act
                sut.Save(configuration, fileName);

                // Assert
                Assert.True(Directory.Exists(folderName));
            }
            finally
            {
                Directory.Delete(folderName, true);
            }
        }

        [Fact]
        public void SaveShouldNotThrowWhenFileCantBeWritten()
        {
            // Arrange
            var serializer = new Serializer();
            var sut = new ConfigurationSerializer(serializer);
            var configuration = new Configuration();
            var fileName = Path.GetRandomFileName();
            try
            {
                using var lockStream = File.Open(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);

                // Act
                var ex = Record.Exception(() => sut.Save(configuration, fileName));

                // Assert
                Assert.Null(ex);
            }
            finally
            {
                File.Delete(fileName);
            }
        }
    }
}