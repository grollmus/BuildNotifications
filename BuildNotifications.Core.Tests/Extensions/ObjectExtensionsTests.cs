using System.Threading.Tasks;
using Xunit;

namespace BuildNotifications.Core.Tests.Extensions
{
    public class ObjectExtensionsTests
    {
        [Fact]
        public async Task AsyncYieldShouldProduceCorrectCollection()
        {
            // Arrange
            var sut = 123;

            // Act
            var actual = sut.AsyncYield();

            // Assert
            var count = 0;
            await foreach (var value in actual)
            {
                ++count;
                Assert.Equal(123, value);
            }

            Assert.Equal(1, count);
        }

        [Fact]
        public void YieldShouldProduceCorrectCollection()
        {
            // Arrange
            var sut = 123;

            // Act
            var actual = sut.Yield();

            // Assert
            Assert.Collection(actual, i => Assert.Equal(123, i));
        }
    }
}