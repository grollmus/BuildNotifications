using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree
{
    public class TreeBuilderTests
    {
        private static TreeBuilder Construct(params GroupDefinition[] definitions)
        {
            var groupDefinition = new BuildTreeGroupDefinition(definitions ?? new GroupDefinition[0]);

            var config = Substitute.For<IConfiguration>();
            config.GroupDefinition.Returns(groupDefinition);

            return new TreeBuilder(config);
        }

        private class BuildTreeParser
        {
            public BuildTreeParser(IBuildTreeNode tree)
            {
                _tree = tree;
            }

            public IEnumerable<IBuildTreeNode> ChildrenAtLevel(int level)
            {
                if (level == 0)
                    _tree.Yield();

                var currentLevel = 1;

                var currentChildren = _tree.Children.ToList();

                while (currentLevel <= level && currentChildren.Any())
                {
                    currentChildren = currentChildren.SelectMany(x => x.Children).ToList();
                    ++currentLevel;
                }

                return currentChildren;
            }

            private readonly IBuildTreeNode _tree;
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
            Assert.Empty(actual.Children);
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
            Assert.Empty(actual.Children);
        }

        [Theory]
        [InlineData(GroupDefinition.Branch)]
        [InlineData(GroupDefinition.BuildDefinition)]
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
            Assert.Equal(expectedCount, actual.Children.Count());
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
            var parser = new BuildTreeParser(actual);

            Assert.All(parser.ChildrenAtLevel(0), x => Assert.IsAssignableFrom<IBuildTree>(x.GetType()));
            Assert.All(parser.ChildrenAtLevel(1), x => Assert.IsAssignableFrom<ISourceGroupNode>(x.GetType()));
            Assert.All(parser.ChildrenAtLevel(2), x => Assert.IsAssignableFrom<IBranchGroupNode>(x.GetType()));
            Assert.All(parser.ChildrenAtLevel(3), x => Assert.IsAssignableFrom<IDefinitionGroupNode>(x.GetType()));
            Assert.All(parser.ChildrenAtLevel(4), x => Assert.IsAssignableFrom<IBuildNode>(x.GetType()));
        }
    }
}