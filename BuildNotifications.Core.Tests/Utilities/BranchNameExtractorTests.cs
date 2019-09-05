using System.Linq;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.SourceControl;
using Xunit;

namespace BuildNotifications.Core.Tests.Utilities
{
    public class BranchNameExtractorTests
    {
        [Theory]
        [InlineData("refs/heads/master", "master")]
        [InlineData("refs/heads/folder/master", "folder/master")]
        [InlineData("refs/pull/123/merge", "PR 123")]
        [InlineData("name", "name")]
        public void ExtractDisplayNameShouldRemoveGitPrefixes(string input, string expected)
        {
            // Arrange
            var sut = new BranchNameExtractor();

            // Act
            var actual = sut.ExtractDisplayName(input, Enumerable.Empty<IBranch>());

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}