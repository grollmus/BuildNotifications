using System.Collections.Generic;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline
{
    public class ProjectFactoryTests
    {
        [Fact]
        public void ConstructShouldReturnNullWhenBuildConnectionDoesNotExist()
        {
            // Arrange
            var pluginRepository = Substitute.For<IPluginRepository>();
            var configuration = Substitute.For<IConfiguration>();

            var sut = new ProjectFactory(pluginRepository, configuration);

            var project = Substitute.For<IProjectConfiguration>();
            project.BuildConnectionNames.Returns(new List<string> {"connection"});

            // Act
            var actual = sut.Construct(project);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void ConstructShouldReturnNullWhenBuildPluginDoesNotExist()
        {
            // Arrange
            var pluginRepository = Substitute.For<IPluginRepository>();
            pluginRepository.FindBuildPlugin("non.existing").Returns((IBuildPlugin?) null);

            var configuration = Substitute.For<IConfiguration>();
            configuration.Connections.Returns(new List<ConnectionData>
            {
                new ConnectionData
                {
                    PluginType = "non.existing",
                    Name = "connection"
                }
            });

            var sut = new ProjectFactory(pluginRepository, configuration);

            var project = Substitute.For<IProjectConfiguration>();
            project.BuildConnectionNames.Returns(new List<string> {"connection"});

            // Act
            var actual = sut.Construct(project);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void ConstructShouldReturnNullWhenSourceConnectionDoesNotExist()
        {
            // Arrange
            var pluginRepository = Substitute.For<IPluginRepository>();
            var configuration = Substitute.For<IConfiguration>();
            configuration.Connections.Returns(new List<ConnectionData>
            {
                new ConnectionData
                {
                    PluginType = "buildPluginType",
                    Name = "connection"
                }
            });

            var sut = new ProjectFactory(pluginRepository, configuration);

            var project = Substitute.For<IProjectConfiguration>();
            project.BuildConnectionNames.Returns(new List<string> {"connection"});
            project.SourceControlConnectionName.Returns("connection2");

            // Act
            var actual = sut.Construct(project);

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void ConstructShouldReturnNullWhenSourceControlPluginDoesNotExist()
        {
            // Arrange
            var pluginRepository = Substitute.For<IPluginRepository>();
            pluginRepository.FindSourceControlPlugin("non.existing").Returns((ISourceControlPlugin?) null);

            var configuration = Substitute.For<IConfiguration>();
            configuration.Connections.Returns(new List<ConnectionData>
            {
                new ConnectionData
                {
                    PluginType = "non.existing",
                    Name = "connection"
                }
            });

            var sut = new ProjectFactory(pluginRepository, configuration);

            var project = Substitute.For<IProjectConfiguration>();
            project.BuildConnectionNames.Returns(new List<string> {"connection"});
            project.SourceControlConnectionName.Returns("connection");

            // Act
            var actual = sut.Construct(project);

            // Assert
            Assert.Null(actual);
        }
    }
}