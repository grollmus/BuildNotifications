using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline;

public class ListBuildFilterTests
{
    private static IBranchNameExtractor BranchNameExtractor()
    {
        var branchNameExtractor = Substitute.For<IBranchNameExtractor>();
        branchNameExtractor.ExtractDisplayName(Arg.Any<string>()).Returns(c => c.ArgAt<string>(0));
        return branchNameExtractor;
    }

    private static IBaseBuild SetupBuild(string branchName, string definitionName)
    {
        var definition = Substitute.For<IBuildDefinition>();
        definition.Name.Returns(definitionName);

        var build = Substitute.For<IBaseBuild>();
        build.Definition.Returns(definition);
        build.BranchName.Returns(branchName);
        return build;
    }

    [Fact]
    public void BuildShouldBAllowedWhenBranchIsNotBlacklisted()
    {
        // Arrange
        var projectConfiguration = Substitute.For<IProjectConfiguration>();
        projectConfiguration.BranchWhitelist.Returns(new[] { "" });
        projectConfiguration.BuildDefinitionWhitelist.Returns(new[] { "" });
        projectConfiguration.BranchBlacklist.Returns(new[] { "branch" });

        var branchNameExtractor = BranchNameExtractor();
        var build = SetupBuild("other branch", "definition");
        branchNameExtractor.ExtractDisplayName(Arg.Any<string>()).Returns(c => c.ArgAt<string>(0));

        var sut = new ListBuildFilter(projectConfiguration, branchNameExtractor);

        // Act
        var actual = sut.IsAllowed(build);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void BuildShouldBeAllowedWhenDefinitionIsNotBlacklisted()
    {
        // Arrange
        var projectConfiguration = Substitute.For<IProjectConfiguration>();
        projectConfiguration.BranchWhitelist.Returns(new[] { "" });
        projectConfiguration.BuildDefinitionWhitelist.Returns(new[] { "" });
        projectConfiguration.BuildDefinitionBlacklist.Returns(new[] { "definition" });

        var branchNameExtractor = BranchNameExtractor();
        var build = SetupBuild("branch", "other definition");

        var sut = new ListBuildFilter(projectConfiguration, branchNameExtractor);

        // Act
        var actual = sut.IsAllowed(build);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void BuildShouldBeAllowedWhenNoBlacklistsExist()
    {
        // Arrange
        var projectConfiguration = Substitute.For<IProjectConfiguration>();

        var branchNameExtractor = BranchNameExtractor();
        var sut = new ListBuildFilter(projectConfiguration, branchNameExtractor);

        var build = SetupBuild("branch", "definition");

        // Act
        var actual = sut.IsAllowed(build);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void BuildShouldNotBeAllowedWhenBranchIsBlacklisted()
    {
        // Arrange
        var projectConfiguration = Substitute.For<IProjectConfiguration>();
        projectConfiguration.BranchBlacklist.Returns(new[] { "branch" });

        var branchNameExtractor = BranchNameExtractor();
        var build = SetupBuild("branch", "definition");

        var sut = new ListBuildFilter(projectConfiguration, branchNameExtractor);

        // Act
        var actual = sut.IsAllowed(build);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void BuildShouldNotBeAllowedWhenDefinitionIsBlacklisted()
    {
        // Arrange
        var projectConfiguration = Substitute.For<IProjectConfiguration>();
        projectConfiguration.BranchWhitelist.Returns(new[] { "" });
        projectConfiguration.BuildDefinitionWhitelist.Returns(new[] { "" });
        projectConfiguration.BuildDefinitionBlacklist.Returns(new[] { "definition" });

        var branchNameExtractor = BranchNameExtractor();
        var build = SetupBuild("branch", "definition");

        var sut = new ListBuildFilter(projectConfiguration, branchNameExtractor);

        // Act
        var actual = sut.IsAllowed(build);

        // Assert
        Assert.True(actual);
    }
}