using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Config
{
    public class ConfigurationBuilderTests
    {
        private static IConfigurationSerializer MockSerializer(Configuration userConfiguration, List<ConnectionData> connectionList)
        {
            var configurationSerializer = Substitute.For<IConfigurationSerializer>();
            configurationSerializer.Load("user").Returns(userConfiguration);
            configurationSerializer.LoadPredefinedConnections("predefined").Returns(connectionList);
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
                new ConnectionData {Name = "c1"},
                new ConnectionData {Name = "c2"}
            };

            var pathResolver = MockResolver();
            var configurationSerializer = MockSerializer(userConfiguration, connectionList);

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
                new ConnectionData {Name = "c1"},
                new ConnectionData {Name = "c2"}
            };

            var pathResolver = MockResolver();
            var configurationSerializer = MockSerializer(userConfiguration, connectionList);

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
                new ConnectionData {Name = "c1"},
                new ConnectionData {Name = "c2"}
            };

            var pathResolver = MockResolver();
            var configurationSerializer = MockSerializer(userConfiguration, connectionList);

            var sut = new ConfigurationBuilder(pathResolver, configurationSerializer);

            // Act
            var config = sut.LoadConfiguration();

            // Assert
            Assert.NotNull(config);

            Assert.Equal(2, config.Connections.Count);
            Assert.Contains(config.Connections.Select(c => c.Name), x => x == "c1");
            Assert.Contains(config.Connections.Select(c => c.Name), x => x == "c2");
        }
    }
}