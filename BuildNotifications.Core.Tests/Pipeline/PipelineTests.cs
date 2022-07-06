using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Cache;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Core.Tests.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using BuildNotifications.TestMocks;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline;

public class PipelineTests
{
    private async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> source)
    {
        await Task.CompletedTask.ConfigureAwait(false);

        foreach (var build in source)
        {
            yield return build;
        }
    }

    private Core.Pipeline.Pipeline MockPipeline(params GroupDefinition[] groupDefinitions)
    {
        var treeBuilder = TreeBuilderTests.Construct(groupDefinitions);

        var masterBranch = new MockBranch("master");
        var ciDefinition = new MockBuildDefinition("1", "ci");
        var stageBranch = new MockBranch("stage");
        var nightlyDefinition = new MockBuildDefinition("2", "nightly");

        var branches = new[] { masterBranch, stageBranch };
        var definitions = new[] { ciDefinition, nightlyDefinition };

        var b1 = Substitute.For<IBuild>();
        b1.Id.Returns("1");
        b1.Definition.Returns(nightlyDefinition);
        b1.BranchName.Returns("stage");

        var b2 = Substitute.For<IBuild>();
        b2.Id.Returns("2");
        b2.Definition.Returns(nightlyDefinition);
        b2.BranchName.Returns("master");

        var configuration = Substitute.For<IConfiguration>();
        configuration.BuildsToShow.Returns(1);

        var buildProvider = Substitute.For<IBuildProvider>();
        buildProvider.FetchAllBuilds(Arg.Any<int>()).Returns(_ => b1.AsyncYield());
        buildProvider.FetchBuildsChangedSince(Arg.Any<DateTime>()).Returns(_ => b2.AsyncYield());
        buildProvider.FetchExistingBuildDefinitions().Returns(_ => ToAsync(definitions));

        var branchProvider = Substitute.For<IBranchProvider>();
        branchProvider.FetchExistingBranches().Returns(_ => ToAsync(branches));
        branchProvider.ExistingBranchCount.Returns(branches.Length);
        var userIdentityList = Substitute.For<IUserIdentityList>();
        var pipeline = new Core.Pipeline.Pipeline(treeBuilder, configuration, userIdentityList);

        var project = new Project(buildProvider, branchProvider, Substitute.For<IProjectConfiguration>(), configuration);
        pipeline.AddProject(project);

        return pipeline;
    }

    [Fact]
    public async Task BranchCacheShouldContainAllBranches()
    {
        // Arrange
        var pipeline = MockPipeline(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);

        // Act
        await pipeline.Update();
        var branches = pipeline.CachedBranches();

        // Assert
        Assert.Equal(new[] { "master", "stage" }, branches.Select(b => b.FullName));
    }

    [Fact]
    public async Task BranchesShouldNotBeMergedWhenComingFromDifferentProviders()
    {
        IBuild BuildOnBranchA()
        {
            var build = Substitute.For<IBuild>();
            build.Id.Returns("1");
            build.Definition.Returns(new MockBuildDefinition());
            build.BranchName.Returns("a");
            return build;
        }

        IBuild BuildOnBranchB()
        {
            var build = Substitute.For<IBuild>();
            build.Id.Returns("2");
            build.Definition.Returns(new MockBuildDefinition());
            build.BranchName.Returns("b");
            return build;
        }

        IBuild BuildOnBranchC()
        {
            var build = Substitute.For<IBuild>();
            build.Id.Returns("3");
            build.Definition.Returns(new MockBuildDefinition());
            build.BranchName.Returns("c");
            return build;
        }

        IBranchProvider BranchProvider(params IBranch[] branches)
        {
            var branchProvider = Substitute.For<IBranchProvider>();
            branchProvider.FetchExistingBranches().Returns(_ => ToAsync(branches));
            branchProvider.ExistingBranchCount.Returns(branches.Length);
            return branchProvider;
        }

        IBuildProvider BuildProvider(params IBuild[] builds)
        {
            var buildProvider = Substitute.For<IBuildProvider>();
            buildProvider.FetchAllBuilds(Arg.Any<int>()).Returns(_ => ToAsync(builds));
            buildProvider.FetchExistingBuildDefinitions().Returns(_ => ToAsync(new[] { new MockBuildDefinition() }));
            return buildProvider;
        }

        // Arrange
        var buildOnBranchA = BuildOnBranchA();
        var buildOnBranchB = BuildOnBranchB();
        var buildOnBranchC = BuildOnBranchC();

        var buildsOfProjectA = new[] { buildOnBranchA, buildOnBranchB };
        var buildsOfProjectB = new[] { buildOnBranchC };

        var configuration = Substitute.For<IConfiguration>();
        configuration.BuildsToShow.Returns(1);

        var treeBuilder = TreeBuilderTests.Construct(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);
        var userIdentityList = Substitute.For<IUserIdentityList>();

        var sut = new Core.Pipeline.Pipeline(treeBuilder, configuration, userIdentityList);

        var branchProviderA = BranchProvider(new MockBranch("a"), new MockBranch("b"));
        var branchProviderB = BranchProvider(new MockBranch("c"));

        var buildProviderA = BuildProvider(buildsOfProjectA);
        var buildProviderB = BuildProvider(buildsOfProjectB);

        var projectA = new Project(buildProviderA, branchProviderA, Substitute.For<IProjectConfiguration>(), configuration)
        {
            Name = "a"
        };
        sut.AddProject(projectA);

        var projectB = new Project(buildProviderB, branchProviderB, Substitute.For<IProjectConfiguration>(), configuration)
        {
            Name = "b"
        };
        sut.AddProject(projectB);

        IBuildTree? tree = null;
        sut.Notifier.Updated += (_, e) => tree = e.Tree;

        // Act
        await sut.Update();

        // Assert
        Assert.NotNull(tree);

        var resultTree = tree!;

        Assert.Equal(2, resultTree.Children.Count());

        var subTreeA = resultTree.Children.OfType<SourceGroupNode>().First(x => x.SourceName == projectA.Name);
        var branchNode = subTreeA.Children.OfType<BranchGroupNode>().First(x => x.BranchName == new MockBranch("a").DisplayName);
        var definitionNode = branchNode.Children.OfType<DefinitionGroupNode>().Single();
        Assert.Single(definitionNode.Children.OfType<BuildNode>());

        branchNode = subTreeA.Children.OfType<BranchGroupNode>().First(x => x.BranchName == new MockBranch("b").DisplayName);
        definitionNode = branchNode.Children.OfType<DefinitionGroupNode>().Single();
        Assert.Single(definitionNode.Children.OfType<BuildNode>());

        var subTreeB = resultTree.Children.OfType<SourceGroupNode>().First(x => x.SourceName == projectB.Name);
        branchNode = subTreeB.Children.OfType<BranchGroupNode>().First(x => x.BranchName == new MockBranch("c").DisplayName);
        definitionNode = branchNode.Children.OfType<DefinitionGroupNode>().Single();
        Assert.Single(definitionNode.Children.OfType<BuildNode>());
    }

    [Fact]
    public async Task BuildsCacheShouldContainAllBuilds()
    {
        // Arrange
        var pipeline = MockPipeline(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);

        // Act
        await pipeline.Update();
        var builds = pipeline.CachedBuilds();

        // Assert
        Assert.Equal(new[] { "1" }, builds.Select(b => b.Id));
    }

    [Fact]
    public void ClearProjectsShouldClearAllLists()
    {
        // Arrange
        var treeBuilder = Substitute.For<ITreeBuilder>();
        var configuration = Substitute.For<IConfiguration>();
        var userIdentityList = Substitute.For<IUserIdentityList>();
        var sut = new Core.Pipeline.Pipeline(treeBuilder, configuration, userIdentityList);

        var buildCache = Substitute.For<IPipelineCache<IBuild>>();
        var branchCache = Substitute.For<IPipelineCache<IBranch>>();
        var definitionCache = Substitute.For<IPipelineCache<IBuildDefinition>>();
        sut.ReplaceCaches(buildCache, branchCache, definitionCache);

        // Act
        sut.ClearProjects();

        // Assert
        userIdentityList.IdentitiesOfCurrentUser.Received(1).Clear();
        buildCache.Received(1).Clear();
        branchCache.Received(1).Clear();
        definitionCache.Received(1).Clear();
    }

    [Fact]
    public async Task DefinitionCacheShouldContainAllDefinitions()
    {
        // Arrange
        var pipeline = MockPipeline(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);

        // Act
        await pipeline.Update();
        var definitions = pipeline.CachedDefinitions();

        // Assert
        Assert.Equal(new[] { "ci", "nightly" }, definitions.Select(d => d.Name));
    }

    [Fact]
    public async Task LastUpdateShouldBeSetAfterUpdate()
    {
        // Arrange
        var pipeline = MockPipeline(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);
        var timeStampBeforeUpdate = pipeline.LastUpdate;

        // Act
        await pipeline.Update();
        var timeStampAfterUpdate = pipeline.LastUpdate;

        // Assert
        Assert.NotEqual(timeStampBeforeUpdate, timeStampAfterUpdate);
    }

    [Fact]
    public async Task UpdateShouldFetchAllBuildDefinitions()
    {
        // Arrange
        var builder = Substitute.For<ITreeBuilder>();
        var configuration = Substitute.For<IConfiguration>();
        var userIdentityList = Substitute.For<IUserIdentityList>();
        var sut = new Core.Pipeline.Pipeline(builder, configuration, userIdentityList);

        var buildProvider = Substitute.For<IBuildProvider>();
        var project = new Project(buildProvider, Substitute.For<IBranchProvider>(), Substitute.For<IProjectConfiguration>(), configuration);
        sut.AddProject(project);

        // Act
        await sut.Update();

        // Assert
        await buildProvider.Received(1).FetchExistingBuildDefinitions().GetAsyncEnumerator().DisposeAsync();
    }

    [Fact]
    public async Task UpdateShouldFetchAllBuildsOnlyWhenCalledTheFirstTime()
    {
        // Arrange
        var builder = Substitute.For<ITreeBuilder>();
        var configuration = Substitute.For<IConfiguration>();
        var userIdentityList = Substitute.For<IUserIdentityList>();
        var sut = new Core.Pipeline.Pipeline(builder, configuration, userIdentityList);

        var buildProvider = Substitute.For<IBuildProvider>();

        var project = new Project(buildProvider, Substitute.For<IBranchProvider>(), Substitute.For<IProjectConfiguration>(), configuration);
        sut.AddProject(project);

        // Act
        await sut.Update();
        await sut.Update();

        // Assert
        await buildProvider.Received(1).FetchAllBuilds(configuration.BuildsToShow).GetAsyncEnumerator().DisposeAsync();
    }

    [Fact]
    public async Task UpdateShouldGroupBuildsCorrectly()
    {
        // Arrange
        var pipeline = MockPipeline(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);
        IBuildTree? tree = null;
        pipeline.Notifier.Updated += (_, e) => tree = e.Tree;

        // Act
        await pipeline.Update();
        await pipeline.Update();

        // Assert
        Assert.NotNull(tree);

        var parser = new BuildTreeParser(tree!);

        Assert.All(parser.ChildrenAtLevel(0), x => Assert.IsAssignableFrom<IBuildTree>(x));
        Assert.All(parser.ChildrenAtLevel(1), x => Assert.IsAssignableFrom<ISourceGroupNode>(x));
        Assert.All(parser.ChildrenAtLevel(2), x => Assert.IsAssignableFrom<IBranchGroupNode>(x));
        Assert.All(parser.ChildrenAtLevel(3), x => Assert.IsAssignableFrom<IDefinitionGroupNode>(x));
        Assert.All(parser.ChildrenAtLevel(4), x => Assert.IsAssignableFrom<IBuildNode>(x));
        Assert.NotEmpty(parser.ChildrenAtLevel(4));
        Assert.Single(parser.ChildrenAtLevel(0));
        Assert.Single(parser.ChildrenAtLevel(1));
        Assert.Equal(2, parser.ChildrenAtLevel(2).Count()); // stage + master
        Assert.Equal(2, parser.ChildrenAtLevel(3).Count()); // ci + nightly
        Assert.Equal(2, parser.ChildrenAtLevel(4).Count()); // 2 builds
    }
}