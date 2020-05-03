using BuildNotifications.Core.Pipeline.Cache;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Cache
{
    public class CacheKeyTests
    {
        [Fact]
        public void IsProviderShouldReturnFalseWhenProviderIsDifferent()
        {
            // Arrange
            var p1 = "123";
            var p2 = "111";

            var sut = new CacheKey(p1, "1");

            // Act
            var actual = sut.IsProvider(p2);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsProviderShouldReturnTrueWhenProviderIsSame()
        {
            // Arrange
            var p1 = "123";
            var p2 = "123";

            var sut = new CacheKey(p1, "1");

            // Act
            var actual = sut.IsProvider(p2);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void KeysShouldBeDifferentWhenDifferentItemIdIsUsed()
        {
            // Arrange
            const string providerId = "1";
            const string itemA = "2";
            const string itemB = "3";

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
            const string providerA = "1";
            const string providerB = "2";
            const string itemId = "3";

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
            const string itemId = "1";
            const string providerId = "2";

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