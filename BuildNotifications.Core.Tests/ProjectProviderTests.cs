using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests
{
    public class ProjectProviderTests
    {
        [Fact]
        public void AllProjectsShouldBeEmptyWhenNoProjectIsConfigured()
        {
            // Arrange
            var pluginRepository = Substitute.For<IPluginRepository>();
            var configuration = new Configuration();

            var sut = new ProjectProvider(configuration, pluginRepository);

            // Act
            var actual = sut.AllProjects().ToList();

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public void AllProjectsShouldContainEveryProjectDefinedInConfiguration()
        {
            // Arrange
            var pluginRepository = Substitute.For<IPluginRepository>();
            var configuration = new Configuration();
            configuration.Projects.Add(new ProjectConfiguration {ProjectName = "p1"});
            configuration.Projects.Add(new ProjectConfiguration {ProjectName = "p2"});
            configuration.Projects.Add(new ProjectConfiguration {ProjectName = "p3"});

            var sut = new ProjectProvider(configuration, pluginRepository);

            // Act
            var actual = sut.AllProjects().ToList();

            // Assert
            Assert.Collection(actual.Select(p => p.Name),
                x => Assert.Equal("p1", x),
                x => Assert.Equal("p2", x),
                x => Assert.Equal("p3", x)
            );
        }
    }
}