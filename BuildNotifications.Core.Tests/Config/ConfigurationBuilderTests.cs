using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Config
{
    public class ConfigurationBuilderTests
    {
        private static IConfigurationSerializer MockSerializer(Configuration userConfiguration, List<ConnectionData> connectionList, List<IProjectConfiguration> projectList)
        {
            var configurationSerializer = Substitute.For<IConfigurationSerializer>();
            configurationSerializer.Load("user").Returns(userConfiguration);
            configurationSerializer.LoadPredefinedConnections("predefined").Returns(connectionList);
            configurationSerializer.LoadPredefinedProjects("predefined").Returns(projectList);
            return configurationSerializer;
        }

        private static IPathResolver MockResolver()
        {
            var pathResolver = Substitute.For<IPathResolver>();
            pathResolver.UserConfigurationFilePath.Returns("user");
            pathResolver.PredefinedConfigurationFilePath.Returns("predefined");
            return pathResolver;
        }

        [Fact]
        public void PredefinedConnectionsShouldBeAddedToConnectionListWhenNoneExists()
        {
            // Arrange
            var userConfiguration = new Configuration();
            var connectionList = new List<ConnectionData>
            {
                new ConnectionData {Name = "c1", ConnectionType = ConnectionPluginType.Build},
                new ConnectionData {Name = "c2", ConnectionType = ConnectionPluginType.SourceControl}
            };
            var projectList = new List<IProjectConfiguration>();

            var pathResolver = MockResolver();
            var configurationSerializer = MockSerializer(userConfiguration, connectionList, projectList);

            var sut = new ConfigurationBuilder(pathResolver, configurationSerializer);

            // Act
            var config = sut.LoadConfiguration();

            // Assert
            Assert.NotNull(config);

            Assert.Equal(2, config.Connections.Count);
            Assert.Contains(config.Connections.Select(c => c.Name), x => x == "c1");
            Assert.Contains(config.Connections.Select(c => c.Name), x => x == "c2");
        }

        [Fact]
        public void PredefinedConnectionsShouldBeAddedToConnectionListWhenOnlyOneExists()
        {
            // Arrange
            var userConfiguration = new Configuration();
            userConfiguration.Connections.Add(new ConnectionData {Name = "c1"});

            var connectionList = new List<ConnectionData>
            {
                new ConnectionData {Name = "c1", ConnectionType = ConnectionPluginType.Build},
                new ConnectionData {Name = "c2", ConnectionType = ConnectionPluginType.SourceControl}
            };
            var projectList = new List<IProjectConfiguration>();

            var pathResolver = MockResolver();
            var configurationSerializer = MockSerializer(userConfiguration, connectionList, projectList);

            var sut = new ConfigurationBuilder(pathResolver, configurationSerializer);

            // Act
            var config = sut.LoadConfiguration();

            // Assert
            Assert.NotNull(config);

            Assert.Equal(2, config.Connections.Count);
            Assert.Contains(config.Connections.Select(c => c.Name), x => x == "c1");
            Assert.Contains(config.Connections.Select(c => c.Name), x => x == "c2");
        }

        [Fact]
        public void PredefinedConnectionsShouldNotBeAddedToConnectionListWhenAllExist()
        {
            // Arrange
            var userConfiguration = new Configuration();
            userConfiguration.Connections.Add(new ConnectionData {Name = "c1"});
            userConfiguration.Connections.Add(new ConnectionData {Name = "c2"});

            var connectionList = new List<ConnectionData>
            {
                new ConnectionData {Name = "c1", ConnectionType = ConnectionPluginType.Build},
                new ConnectionData {Name = "c2", ConnectionType = ConnectionPluginType.SourceControl}
            };
            var projectList = new List<IProjectConfiguration>();

            var pathResolver = MockResolver();
            var configurationSerializer = MockSerializer(userConfiguration, connectionList, projectList);

            var sut = new ConfigurationBuilder(pathResolver, configurationSerializer);

            // Act
            var config = sut.LoadConfiguration();

            // Assert
            Assert.NotNull(config);

            Assert.Equal(2, config.Connections.Count);
            Assert.Contains(config.Connections.Select(c => c.Name), x => x == "c1");
            Assert.Contains(config.Connections.Select(c => c.Name), x => x == "c2");
        }

        [Fact]
        public void PredefinedProjectsShouldBeAddedToProjectList()
        {
            // Arrange
            var userConfiguration = new Configuration();
            var connectionList = new List<ConnectionData>
            {
                new ConnectionData {Name = "c1", ConnectionType = ConnectionPluginType.Build},
                new ConnectionData {Name = "c2", ConnectionType = ConnectionPluginType.SourceControl}
            };
            var projectList = new List<IProjectConfiguration>
            {
                new ProjectConfiguration {ProjectName = "p1"},
                new ProjectConfiguration {ProjectName = "p2"}
            };

            var pathResolver = MockResolver();
            var configurationSerializer = MockSerializer(userConfiguration, connectionList, projectList);

            var sut = new ConfigurationBuilder(pathResolver, configurationSerializer);

            // Act
            var config = sut.LoadConfiguration();

            // Assert
            Assert.NotNull(config);

            Assert.Equal(2, config.Projects.Count);
            Assert.Contains(config.Projects.Select(c => c.ProjectName), x => x == "p1");
            Assert.Contains(config.Projects.Select(c => c.ProjectName), x => x == "p2");
        }

        [Fact]
        public void PredefinedProjectsShouldContainCorrectConnections()
        {
            var userConfiguration = new Configuration();
            var connectionList = new List<ConnectionData>
            {
                new ConnectionData {Name = "c1", ConnectionType = ConnectionPluginType.Build},
                new ConnectionData {Name = "c2", ConnectionType = ConnectionPluginType.SourceControl}
            };
            var projectList = new List<IProjectConfiguration>
            {
                new ProjectConfiguration {ProjectName = "p1", BuildConnectionNames = new[] {"c1"}, SourceControlConnectionName = "c2"},
                new ProjectConfiguration {ProjectName = "p2", BuildConnectionNames = new[] {"c1", "c2"}, SourceControlConnectionName = "c1"}
            };

            var pathResolver = MockResolver();
            var configurationSerializer = MockSerializer(userConfiguration, connectionList, projectList);

            var sut = new ConfigurationBuilder(pathResolver, configurationSerializer);

            // Act
            var config = sut.LoadConfiguration();

            // Assert
            Assert.NotNull(config);
            Assert.Equal(2, config.Projects.Count);

            var p1 = config.Projects.First(p => p.ProjectName == "p1");
            var p2 = config.Projects.First(p => p.ProjectName == "p2");

            Assert.Contains("c1", p1.BuildConnectionNames);
            Assert.Single(p1.BuildConnectionNames);
            Assert.Equal("c2", p1.SourceControlConnectionName);

            Assert.Contains("c1", p2.BuildConnectionNames);
            Assert.Contains("c2", p2.BuildConnectionNames);
            Assert.Equal(2, p2.BuildConnectionNames.Count);
            Assert.Equal("c1", p2.SourceControlConnectionName);
        }
    }
}