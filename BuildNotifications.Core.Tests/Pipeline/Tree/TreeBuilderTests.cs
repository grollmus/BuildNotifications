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
    }
}