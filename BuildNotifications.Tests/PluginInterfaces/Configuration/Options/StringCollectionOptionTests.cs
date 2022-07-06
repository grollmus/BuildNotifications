using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options;

public class StringCollectionOptionTests
{
    public static TheoryData<IEnumerable<string>> TestCases => new()
    {
        new[] { "one", "two", "three" },
        new[] { "1", "2", "3" },
        Enumerable.Empty<string>()
    };

    [Theory]
    [MemberData(nameof(TestCases))]
    public void ConstructorShouldSetCorrectValue(string[] expected)
    {
        // Arrange
        var sut = new StringCollectionOption(expected, "name", "description");

        // Act
        var actual = sut.Value;

        // Assert
        Assert.Equal(expected, actual);
    }
}