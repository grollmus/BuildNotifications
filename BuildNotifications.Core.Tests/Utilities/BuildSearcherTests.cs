using System.Diagnostics.CodeAnalysis;
using BuildNotifications.Core.Utilities;
using Xunit;

namespace BuildNotifications.Core.Tests.Utilities
{
    [SuppressMessage("ReSharper", "StringLiteralTypo")]
    public class BuildSearcherTests
    {
        [Theory]
        [InlineData("definition", "branch", "definitionbranch")]
        [InlineData("definition", "branch", "something completely different")]
        public void MatchShouldReturnFalseForNonMatchingTerms(string definitionName, string branchName, string searchTerm)
        {
            // Arrange
            var sut = new BuildSearcher();

            var definition = new MockBuildDefinition("id", definitionName);
            var build = new MockBuild("id", definition, branchName);

            // Act
            var actual = sut.Matches(build, searchTerm);

            // Assert
            Assert.False(actual);
        }

        [Theory]
        [InlineData("definition", "branch", "definition")]
        [InlineData("definition", "branch", "Definition")]
        [InlineData("Definition", "branch", "definition")]
        [InlineData("definition", "branch", "branch")]
        [InlineData("definition", "branch", "Branch")]
        [InlineData("definition", "Branch", "branch")]
        public void MatchShouldReturnTrueForMatchingTerms(string definitionName, string branchName, string searchTerm)
        {
            // Arrange
            var sut = new BuildSearcher();

            var definition = new MockBuildDefinition("id", definitionName);
            var build = new MockBuild("id", definition, branchName);

            // Act
            var actual = sut.Matches(build, searchTerm);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("definition", "branch", "bra")]
        [InlineData("definition", "branch", "anc")]
        [InlineData("definition", "branch", "nch")]
        [InlineData("definition", "branch", "def")]
        [InlineData("definition", "branch", "ini")]
        [InlineData("definition", "branch", "ion")]
        public void MatchShouldReturnTrueForTermsContained(string definitionName, string branchName, string searchTerm)
        {
            // Arrange
            var sut = new BuildSearcher();

            var definition = new MockBuildDefinition("id", definitionName);
            var build = new MockBuild("id", definition, branchName);

            // Act
            var actual = sut.Matches(build, searchTerm);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        public void MatchShouldReturnTrueWhenSearchTermIsEmpty(string searchTerm)
        {
            // Arrange
            var sut = new BuildSearcher();

            var definition = new MockBuildDefinition("id", "definition");
            var build = new MockBuild("id", definition, "branch");

            // Act
            var actual = sut.Matches(build, searchTerm);

            // Assert
            Assert.True(actual);
        }
    }
}