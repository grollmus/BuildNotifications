using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options
{
    public class DisplayOptionTests
    {
        [Theory]
        [InlineData("")]
        [InlineData("test")]
        [InlineData("Hello, World!")]
        public void ConstructorShouldSetCorrectValue(string expected)
        {
            // Arrange
            var sut = new DisplayOption(expected, "name", "description");

            // Act
            var actual = sut.Value;

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}