using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options;

public class BooleanOptionTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ConstructorShouldSetCorrectValue(bool expected)
    {
        // Arrange
        var sut = new BooleanOption(expected, "name", "description");

        // Act
        var actual = sut.Value;

        // Assert
        Assert.Equal(expected, actual);
    }
}