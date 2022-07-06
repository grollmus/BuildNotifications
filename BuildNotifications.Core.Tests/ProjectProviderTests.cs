using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests;

public class ProjectProviderTests
{
    public ProjectProviderTests()
    {
        _sourceConnection = new ConnectionData { Name = "source" };

        _buildConnection = new ConnectionData { Name = "build" };
    }

    private readonly ConnectionData _sourceConnection;
    private readonly ConnectionData _buildConnection;

    private ProjectConfiguration CreateProjectConfiguration(string name, bool enabled = true)
    {
        return new ProjectConfiguration
        {
            ProjectName = name,
            IsEnabled = enabled,
            SourceControlConnectionName = "source",
            BuildConnectionNames = new[] { "build" }
        };
    }

    private Configuration CreateConfiguration()
    {
        var configuration = new Configuration();
        configuration.Connections.Add(_sourceConnection);
        configuration.Connections.Add(_buildConnection);
        return configuration;
    }

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
    public void AllProjectsShouldContainDisabledProjectsDefinedInConfiguration()
    {
        // Arrange
        var pluginRepository = Substitute.For<IPluginRepository>();
        var configuration = CreateConfiguration();
        configuration.Projects.Add(CreateProjectConfiguration("p1", false));
        configuration.Projects.Add(CreateProjectConfiguration("p2"));
        configuration.Projects.Add(CreateProjectConfiguration("p3", false));

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

    [Fact]
    public void AllProjectsShouldContainEveryProjectDefinedInConfiguration()
    {
        // Arrange
        var pluginRepository = Substitute.For<IPluginRepository>();
        var configuration = CreateConfiguration();
        configuration.Projects.Add(CreateProjectConfiguration("p1"));
        configuration.Projects.Add(CreateProjectConfiguration("p2"));
        configuration.Projects.Add(CreateProjectConfiguration("p3"));

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

    [Fact]
    public void EnabledProjectsShouldNotContainDisabledProjectsDefinedInConfiguration()
    {
        // Arrange
        var pluginRepository = Substitute.For<IPluginRepository>();
        var configuration = CreateConfiguration();
        configuration.Projects.Add(CreateProjectConfiguration("p1", false));
        configuration.Projects.Add(CreateProjectConfiguration("p2"));
        configuration.Projects.Add(CreateProjectConfiguration("p3"));

        var sut = new ProjectProvider(configuration, pluginRepository);

        // Act
        var actual = sut.EnabledProjects().ToList();

        // Assert
        Assert.Collection(actual.Select(p => p.Name),
            x => Assert.Equal("p2", x),
            x => Assert.Equal("p3", x)
        );
    }
}