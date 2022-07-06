using BuildNotifications.Core.Pipeline;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline;

public class NullBranchNameExtractorTests
{
    [Fact]
    public void ExtractDisplayNameShouldReturnGivenName()
    {
        // Arrange
        var sut = new NullBranchNameExtractor();

        // Act
        var actual = sut.ExtractDisplayName("test");

        // Assert
        Assert.Equal("test", actual);
    }
}