using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options
{
    public class ValueCalculationResultTests
    {
        [Fact]
        public void FailShouldCreateFailedResult()
        {
            // Act
            var actual = ValueCalculationResult.Fail<int>();

            // Assert
            Assert.NotNull(actual);
            Assert.False(actual.Success);
        }

        [Fact]
        public void SuccessShouldCreateResultWithCorrectValue()
        {
            // Arrange
            var expected = 123;

            // Act
            var actual = ValueCalculationResult.Success(expected);

            // Assert
            Assert.NotNull(actual);
            Assert.True(actual.Success);
            Assert.Equal(expected, actual.Value);
        }
    }
}