using Xunit;

namespace BuildNotifications.Core.Tests.Extensions;

public class EnumerableExtensionsTests
{
    [Fact]
    public void FlattenShouldProduceFlatList()
    {
        // Arrange
        var sut = new[]
        {
            new[] { 1, 2, 3 },
            new[] { 4, 5, 6 },
            new[] { 7, 8, 9 }
        };

        // Act
        var actual = sut.Flatten();

        // Assert
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void WhereNotNullShouldOnlyReturnNonNullValues()
    {
        // Arrange
        var source = new[]
        {
            "a", "b", null, "c"
        };

        // Act
        var actual = source.WhereNotNull();

        // Assert
        Assert.Collection(actual,
            s0 => Assert.Equal("a", s0),
            s1 => Assert.Equal("b", s1),
            s2 => Assert.Equal("c", s2)
        );
    }
}