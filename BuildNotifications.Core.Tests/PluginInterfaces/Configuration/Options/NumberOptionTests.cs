using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Core.Tests.PluginInterfaces.Configuration.Options
{
    public class NumberOptionTests
    {
        [Theory]
        [InlineData(10, 0, 20, true)]
        [InlineData(10, 11, 20, false)]
        [InlineData(11, 11, 20, true)]
        [InlineData(21, 11, 20, false)]
        [InlineData(20, 11, 20, true)]
        public void SettingValueShouldOnlySucceedWhenValueIsInValidRange(int value, int min, int max, bool inRange)
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
            if (inRange)
                Assert.Equal(value, sut.Value);
            else
                Assert.NotEqual(value, sut.Value);
        }
    }
}