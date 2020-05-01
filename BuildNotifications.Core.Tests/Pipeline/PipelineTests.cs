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
            await Task.CompletedTask.ConfigureAwait(false);

            foreach (var build in source)
            {
                yield return build;
            }
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
            var pipeline = MockPipeline(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);
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
            Assert.Equal(2, parser.ChildrenAtLevel(2).Count()); // stage + master
            Assert.Equal(2, parser.ChildrenAtLevel(3).Count()); // ci + nightly
            Assert.Equal(2, parser.ChildrenAtLevel(4).Count()); // 2 builds
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
        public async Task BuildsCacheShouldContainAllBuilds()
        {
            // Arrange
            var pipeline = MockPipeline(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);

            // Act
            await pipeline.Update();
            var builds = pipeline.CachedBuilds();

            // Assert
            Assert.Equal(new [] {"1"}, builds.Select(b => b.Id));
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
            Assert.Equal(new [] {"ci", "nightly"}, definitions.Select(d => d.Name));
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
            Assert.Equal(new [] {"master", "stage"}, branches.Select(b => b.FullName));
        }

        private Core.Pipeline.Pipeline MockPipeline(params GroupDefinition[] groupDefinitions)
        {
            var treeBuilder = TreeBuilderTests.Construct(groupDefinitions);

            var masterBranch = new MockBranch("master");
            var ciDefinition = new MockBuildDefinition("1", "ci");
            var stageBranch = new MockBranch("stage");
            var nightlyDefinition = new MockBuildDefinition("2", "nightly");

            var branches = new[] {masterBranch, stageBranch};
            var definitions = new[] {ciDefinition, nightlyDefinition};

            var b1 = Substitute.For<IBuild>();
            b1.Id.Returns("1");
            b1.Definition.Returns(nightlyDefinition);
            b1.BranchName.Returns("stage");

            var b2 = Substitute.For<IBuild>();
            b2.Id.Returns("2");
            b2.Definition.Returns(nightlyDefinition);
            b2.BranchName.Returns("master");

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

            return pipeline;
        }
    }
}