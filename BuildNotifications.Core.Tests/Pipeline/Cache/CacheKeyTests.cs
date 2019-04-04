using BuildNotifications.Core.Pipeline.Cache;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Cache
{
    public class CacheKeyTests
    {
        [Fact]
        public void KeysShouldBeDifferentWhenDifferentItemIdIsUsed()
        {
            // Arrange
            const int providerId = 1;
            const int itemA = 2;
            const int itemB = 3;

            var a = new CacheKey(providerId, itemA);
            var b = new CacheKey(providerId, itemB);

            // Act
            var ab = a.Equals(b);
            var ba = b.Equals(a);

            // Assert
            Assert.False(ab);
            Assert.False(ba);
        }

        [Fact]
        public void KeysShouldBeDifferentWhenDifferentProviderIsUsed()
        {
            // Arrange
            const int providerA = 1;
            const int providerB = 2;
            const int itemId = 3;

            var a = new CacheKey(providerA, itemId);
            var b = new CacheKey(providerB, itemId);

            // Act
            var ab = a.Equals(b);
            var ba = b.Equals(a);

            // Assert
            Assert.False(ab);
            Assert.False(ba);
        }

        [Fact]
        public void KeysShouldBeEqualWhenItemAndProviderAreEqual()
        {
            // Arrange
            const int itemId = 1;
            const int providerId = 2;

            var a = new CacheKey(providerId, itemId);
            var b = new CacheKey(providerId, itemId);

            // Act
            var ab = a.Equals(b);
            var ba = b.Equals(a);

            // Assert
            Assert.True(ab);
            Assert.True(ba);
        }
    }
}