using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Core.Tests.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline
{
    public class PipelineTests
    {
        private async IAsyncEnumerable<T> ToAsync<T>(IEnumerable<T> source)
        {
            await Task.CompletedTask;

            foreach (var build in source)
            {
                yield return build;
            }
        }

        [Fact]
        public async Task BranchProviderShouldNotBeCalledMultipleTimesWhenDefinedInMultipleProjects()
        {
            // Arrange
            var builder = Substitute.For<ITreeBuilder>();
            var configuration = Substitute.For<IConfiguration>();
            var sut = new Core.Pipeline.Pipeline(builder, configuration);

            var branchProvider = Substitute.For<IBranchProvider>();

            var project1 = new Project(Substitute.For<IBuildProvider>(), branchProvider, Substitute.For<IProjectConfiguration>());
            var project2 = new Project(Substitute.For<IBuildProvider>(), branchProvider, Substitute.For<IProjectConfiguration>());

            sut.AddProject(project1);
            sut.AddProject(project2);

            // Act
            await sut.Update();

            // Assert
            await branchProvider.Received(1).FetchExistingBranches().GetAsyncEnumerator().DisposeAsync();
        }

        [Fact]
        public async Task BuildProviderShouldNotBeCalledMultipleTimesWhenDefinedInMultipleProjects()
        {
            // Arrange
            var builder = Substitute.For<ITreeBuilder>();
            var configuration = Substitute.For<IConfiguration>();
            var sut = new Core.Pipeline.Pipeline(builder, configuration);

            var buildProvider = Substitute.For<IBuildProvider>();
            var project1 = new Project(buildProvider, Substitute.For<IBranchProvider>(), Substitute.For<IProjectConfiguration>());
            var project2 = new Project(buildProvider, Substitute.For<IBranchProvider>(), Substitute.For<IProjectConfiguration>());

            sut.AddProject(project1);
            sut.AddProject(project2);

            // Act
            await sut.Update();

            // Assert
            await buildProvider.Received(1).FetchExistingBuildDefinitions().GetAsyncEnumerator().DisposeAsync();
        }

        [Fact]
        public async Task UpdateShouldFetchAllBuildDefinitions()
        {
            // Arrange
            var builder = Substitute.For<ITreeBuilder>();
            var configuration = Substitute.For<IConfiguration>();
            var sut = new Core.Pipeline.Pipeline(builder, configuration);

            var buildProvider = Substitute.For<IBuildProvider>();
            var project = new Project(buildProvider, Substitute.For<IBranchProvider>(), Substitute.For<IProjectConfiguration>());
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
            var sut = new Core.Pipeline.Pipeline(builder, configuration);

            var buildProvider = Substitute.For<IBuildProvider>();

            var project = new Project(buildProvider, Substitute.For<IBranchProvider>(), Substitute.For<IProjectConfiguration>());
            sut.AddProject(project);

            // Act
            await sut.Update();
            await sut.Update();

            // Assert
            await buildProvider.Received(1).FetchAllBuilds().GetAsyncEnumerator().DisposeAsync();
        }

        [Fact]
        public async Task UpdateShouldGroupBuildsCorrectly()
        {
            // Arrange
            var treeBuilder = TreeBuilderTests.Construct(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);

            var masterBranch = Substitute.For<IBranch>();
            var ciDefinition = Substitute.For<IBuildDefinition>();
            var stageBranch = Substitute.For<IBranch>();
            var nightlyDefinition = Substitute.For<IBuildDefinition>();

            var branches = new[] {masterBranch, stageBranch};
            var definitions = new[] {ciDefinition, nightlyDefinition};

            var b1 = Substitute.For<IBuild>();
            b1.Id.Returns("1");
            b1.Definition.Returns(nightlyDefinition);
            b1.BranchName.Returns("stage");

            var b2 = Substitute.For<IBuild>();
            b2.Id.Returns("2");
            b2.Definition.Returns(nightlyDefinition);
            b2.BranchName.Returns("stage");

            var buildProvider = Substitute.For<IBuildProvider>();
            buildProvider.FetchAllBuilds().Returns(x => b1.AsyncYield());
            buildProvider.FetchBuildsChangedSince(Arg.Any<DateTime>()).Returns(x => b2.AsyncYield());
            buildProvider.FetchExistingBuildDefinitions().Returns(x => ToAsync(definitions));

            var branchProvider = Substitute.For<IBranchProvider>();
            branchProvider.FetchExistingBranches().Returns(x => ToAsync(branches));

            var configuration = Substitute.For<IConfiguration>();
            configuration.BuildsToShow.Returns(int.MaxValue);
            var pipeline = new Core.Pipeline.Pipeline(treeBuilder, configuration);

            var project = new Project(buildProvider, branchProvider, Substitute.For<IProjectConfiguration>());
            pipeline.AddProject(project);

            IBuildTree? tree = null;
            pipeline.Notifier.Updated += (s, e) => tree = e.Tree;

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
            Assert.Single(parser.ChildrenAtLevel(2));
            Assert.Single(parser.ChildrenAtLevel(3));
            Assert.Equal(2, parser.ChildrenAtLevel(4).Count());
        }
    }
}