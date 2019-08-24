using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree
{
    internal class BuildTreeParser
    {
        public BuildTreeParser(IBuildTreeNode tree)
        {
            _tree = tree;
        }

        public IEnumerable<IBuildTreeNode> ChildrenAtLevel(int level)
        {
            if (level == 0)
                return _tree.Yield();

            var currentLevel = 1;

            var currentChildren = _tree.Children.ToList();

            while (currentLevel < level && currentChildren.Any())
            {
                currentChildren = currentChildren.SelectMany(x => x.Children).ToList();
                ++currentLevel;
            }

            return currentChildren;
        }

        private readonly IBuildTreeNode _tree;
    }

    public class TreeBuilderTests
    {
        internal static TreeBuilder Construct(params GroupDefinition[] definitions)
        {
            var groupDefinition = new BuildTreeGroupDefinition(definitions ?? new GroupDefinition[0]);

            var config = Substitute.For<IConfiguration>();
            config.GroupDefinition.Returns(groupDefinition);

            var branchNameExtractor = Substitute.For<IBranchNameExtractor>();

            return new TreeBuilder(config, branchNameExtractor);
        }

        [Fact]
        public void BuildShouldCreateEmptyTreeWhenDefinitionIsEmpty()
        {
            // Arrange
            var sut = Construct();

            var builds = Enumerable.Empty<IBuild>();
            var branches = Enumerable.Empty<IBranch>();
            var definitions = Enumerable.Empty<IBuildDefinition>();

            // Act
            var actual = sut.Build(builds, branches, definitions);

            // Assert
            Assert.Empty(actual.Tree.Children);
        }

        [Theory]
        [InlineData(new[] {GroupDefinition.Branch, GroupDefinition.BuildDefinition, GroupDefinition.Status})]
        [InlineData(new[] {GroupDefinition.BuildDefinition, GroupDefinition.Branch, GroupDefinition.Status})]
        public void BuildShouldCreateEmptyTreeWhenInputsAreEmpty(GroupDefinition[] groupDefinitions)
        {
            // Arrange
            var sut = Construct(groupDefinitions);

            var builds = Enumerable.Empty<IBuild>();
            var branches = Enumerable.Empty<IBranch>();
            var definitions = Enumerable.Empty<IBuildDefinition>();

            // Act
            var actual = sut.Build(builds, branches, definitions);

            // Assert
            Assert.Empty(actual.Tree.Children);
        }

        [Theory]
        [InlineData(GroupDefinition.Branch)]
        [InlineData(GroupDefinition.BuildDefinition)]
        [InlineData(GroupDefinition.None)]
        public void BuildShouldCreateFlatListWhenGroupingByOneCriteria(GroupDefinition grouping)
        {
            // Arrange
            var sut = Construct(grouping, GroupDefinition.Status);

            var branch = Substitute.For<IBranch>();
            var definition = Substitute.For<IBuildDefinition>();

            var branches = new[] {branch};
            var definitions = new[] {definition};

            var builds = new List<IBuild>();

            var b1 = Substitute.For<IBuild>();
            b1.Definition.Returns(definition);
            b1.BranchName.Returns("branch");
            builds.Add(b1);

            // Act
            var actual = sut.Build(builds, branches, definitions);

            // Assert
            var expectedCount = builds.Count;
            Assert.Equal(expectedCount, actual.Tree.Children.Count());
        }

        [Fact]
        public void BuildTreeShouldMatchGroupDefinition()
        {
            // Arrange
            var sut = Construct(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);

            var masterBranch = Substitute.For<IBranch>();
            var ciDefinition = Substitute.For<IBuildDefinition>();
            var stageBranch = Substitute.For<IBranch>();
            var nightlyDefinition = Substitute.For<IBuildDefinition>();

            var branches = new[] {masterBranch, stageBranch};
            var definitions = new[] {ciDefinition, nightlyDefinition};

            var builds = new List<IBuild>();

            var b1 = Substitute.For<IBuild>();
            b1.Definition.Returns(nightlyDefinition);
            b1.BranchName.Returns("stage");
            builds.Add(b1);

            // Act
            var actual = sut.Build(builds, branches, definitions);

            // Assert
            var parser = new BuildTreeParser(actual.Tree);

            Assert.All(parser.ChildrenAtLevel(0), x => Assert.IsAssignableFrom<IBuildTree>(x));
            Assert.All(parser.ChildrenAtLevel(1), x => Assert.IsAssignableFrom<ISourceGroupNode>(x));
            Assert.All(parser.ChildrenAtLevel(2), x => Assert.IsAssignableFrom<IBranchGroupNode>(x));
            Assert.All(parser.ChildrenAtLevel(3), x => Assert.IsAssignableFrom<IDefinitionGroupNode>(x));
            Assert.All(parser.ChildrenAtLevel(4), x => Assert.IsAssignableFrom<IBuildNode>(x));
            Assert.NotEmpty(parser.ChildrenAtLevel(4));
        }
        
        [Theory]
        [InlineData(BuildStatus.Succeeded)]
        [InlineData(BuildStatus.PartiallySucceeded)]
        [InlineData(BuildStatus.Failed)]
        [InlineData(BuildStatus.Cancelled)]
        public void BuildTreeWithOneBuildWithUpdatedStatusShouldCreateDelta(BuildStatus expectedResult)
        {
            // Arrange
            var sut = Construct(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);

            var masterBranch = Substitute.For<IBranch>();
            var ciDefinition = Substitute.For<IBuildDefinition>();
            var stageBranch = Substitute.For<IBranch>();
            var nightlyDefinition = Substitute.For<IBuildDefinition>();

            var branches = new[] {masterBranch, stageBranch};
            var definitions = new[] {ciDefinition, nightlyDefinition};

            var builds = new List<IBuild>();

            var build1 = CreateBuild(ciDefinition, stageBranch, "1");
            builds.Add(build1);

            var build2 = CreateBuild(ciDefinition, stageBranch, "2");
            builds.Add(build2);

            var build3 = CreateBuild(ciDefinition, stageBranch, "3");
            builds.Add(build3);

            // Act
            var firstResult = sut.Build(builds, branches, definitions);
            var newBuild2 = CreateBuild(ciDefinition, stageBranch, "2");
            newBuild2.Status.Returns(expectedResult);

            var updatedBuilds = new List<IBuild> {build1, newBuild2, build3};

            var result = sut.Build(updatedBuilds, branches, definitions, firstResult.Tree);

            // Assert
            var delta = result.Delta;
            switch (expectedResult)
            {
                case BuildStatus.Cancelled:
                    Assert.Equal(delta.Cancelled.Count(), 1);
                    break;
                case BuildStatus.Succeeded:
                case BuildStatus.PartiallySucceeded:
                    Assert.Equal(delta.Succeeded.Count(), 1);
                    break;
                case BuildStatus.Failed:
                    Assert.Equal(delta.Failed.Count(), 1);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expectedResult), expectedResult, null);
            }
        }
        
        [Theory]
        [InlineData(BuildStatus.Succeeded)]
        [InlineData(BuildStatus.PartiallySucceeded)]
        [InlineData(BuildStatus.Failed)]
        [InlineData(BuildStatus.Cancelled)]
        public void BuildTreeWithUpdatesStatusShouldNotProduceDeltaForDifferentStatus(BuildStatus expectedResult)
        {
            // Arrange
            var sut = Construct(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);

            var masterBranch = Substitute.For<IBranch>();
            var ciDefinition = Substitute.For<IBuildDefinition>();
            var stageBranch = Substitute.For<IBranch>();
            var nightlyDefinition = Substitute.For<IBuildDefinition>();

            var branches = new[] {masterBranch, stageBranch};
            var definitions = new[] {ciDefinition, nightlyDefinition};

            var builds = new List<IBuild>();

            var build1 = CreateBuild(ciDefinition, stageBranch, "1");
            builds.Add(build1);

            var build2 = CreateBuild(ciDefinition, stageBranch, "2");
            builds.Add(build2);

            var build3 = CreateBuild(ciDefinition, stageBranch, "3");
            builds.Add(build3);

            // Act
            var firstResult = sut.Build(builds, branches, definitions);
            var newBuild2 = CreateBuild(ciDefinition, stageBranch, "2");
            newBuild2.Status.Returns(expectedResult);

            var updatedBuilds = new List<IBuild> {build1, newBuild2, build3};

            var result = sut.Build(updatedBuilds, branches, definitions, firstResult.Tree);

            // Assert
            var delta = result.Delta;
            switch (expectedResult)
            {
                case BuildStatus.Cancelled:
                    Assert.Equal(delta.Succeeded.Count(), 0);
                    Assert.Equal(delta.Failed.Count(), 0);
                    break;
                case BuildStatus.Succeeded:
                case BuildStatus.PartiallySucceeded:
                    Assert.Equal(delta.Failed.Count(), 0);
                    Assert.Equal(delta.Cancelled.Count(), 0);
                    break;
                case BuildStatus.Failed:
                    Assert.Equal(delta.Succeeded.Count(), 0);
                    Assert.Equal(delta.Cancelled.Count(), 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(expectedResult), expectedResult, null);
            }
        }
        
        [Fact]
        public void BuildTreeWithMultipleBuildsChangingStatusShouldCreateDelta()
        {
            // Arrange
            var sut = Construct(GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition);

            var masterBranch = Substitute.For<IBranch>();
            var ciDefinition = Substitute.For<IBuildDefinition>();
            var stageBranch = Substitute.For<IBranch>();
            var nightlyDefinition = Substitute.For<IBuildDefinition>();

            var branches = new[] {masterBranch, stageBranch};
            var definitions = new[] {ciDefinition, nightlyDefinition};

            var builds = new List<IBuild>();

            var build1 = CreateBuild(ciDefinition, stageBranch, "1");
            builds.Add(build1);

            var build2 = CreateBuild(ciDefinition, stageBranch, "2");
            builds.Add(build2);

            var build3 = CreateBuild(ciDefinition, stageBranch, "3");
            builds.Add(build3);

            // Act
            var firstResult = sut.Build(builds, branches, definitions);
            var newBuild1 = CreateBuild(ciDefinition, stageBranch, "1");
            newBuild1.Status.Returns(BuildStatus.Failed);

            var newBuild2 = CreateBuild(ciDefinition, stageBranch, "2");
            newBuild2.Status.Returns(BuildStatus.Succeeded);

            var newBuild3 = CreateBuild(ciDefinition, stageBranch, "3");
            newBuild3.Status.Returns(BuildStatus.Cancelled);

            var updatedBuilds = new List<IBuild> {newBuild1, newBuild2, newBuild3};

            var result = sut.Build(updatedBuilds, branches, definitions, firstResult.Tree);

            // Assert
            var delta = result.Delta;
            Assert.Equal(delta.Failed.Count(), 1);
            Assert.Equal(delta.Cancelled.Count(), 1);
            Assert.Equal(delta.Succeeded.Count(), 1);
        }
        
        private IBuild CreateBuild(IBuildDefinition definition, IBranch branch, string id)
        {
            var build = Substitute.For<IBuild>();
            build.Definition.Returns(definition);
            build.BranchName.Returns(branch.Name);
            build.Id.Returns(id);

            return build;
        }
    }
}