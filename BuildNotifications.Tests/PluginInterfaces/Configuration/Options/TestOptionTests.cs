using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options;

public class TextOptionTests
{
    [Theory]
    [InlineData("a", 0, 1, true)]
    [InlineData("a", 1, 3, true)]
    [InlineData("aa", 1, 3, true)]
    [InlineData("aaa", 1, 3, true)]
    [InlineData("aaaa", 1, 3, false)]
    [InlineData("a", 2, 3, false)]
    public void SettingValueShouldOnlySucceedWhenValueIsInRange(string value, int minLength, int maxLength, bool inRange)
    {
        // Arrange
        var sut = new TextOption(string.Empty, string.Empty, string.Empty)
        {
            MaximumLength = maxLength,
            MinimumLength = minLength
        };

        // Act
        sut.Value = value;

        // Assert
        if (inRange)
            Assert.Equal(value, sut.Value);
        else
            Assert.NotEqual(value, sut.Value);
    }
}