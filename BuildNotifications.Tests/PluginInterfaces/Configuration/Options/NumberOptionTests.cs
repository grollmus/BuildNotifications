using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options;

public class NumberOptionTests
{
    [Theory]
    [InlineData(10, 0, 20, 10)]
    [InlineData(11, 11, 20, 11)]
    [InlineData(20, 11, 20, 20)]
    [InlineData(0, 1, 20, 1)]
    [InlineData(0, 0, 20, 0)]
    [InlineData(11, 0, 10, 10)]
    [InlineData(10, 0, 10, 10)]
    public void SettingValueShouldClampValuesBetweenMinAndMax(int value, int min, int max, int expected)
    {
        // Arrange
        var sut = new NumberOption(0, string.Empty, string.Empty)
        {
            MinValue = min,
            MaxValue = max
        };

        // Act
        sut.Value = value;

        // Assert
        Assert.Equal(expected, sut.Value);
    }
}