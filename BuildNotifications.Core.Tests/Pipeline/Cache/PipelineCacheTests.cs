using System.Linq;
using BuildNotifications.Core.Pipeline.Cache;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Cache
{
    public class PipelineCacheTests
    {
        [Fact]
        public void AddShouldReplaceExistingItem()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            var key = new CacheKey(1, 2);
            sut.AddOrReplace(key, "hello world");

            // Act
            sut.AddOrReplace(key, "test");

            // Assert
            var content = sut.ContentCopy().ToList();

            Assert.Collection(content, str => Assert.Equal("test", str));
        }

        [Fact]
        public void AddShouldReturnFalseWhenItemWasAlreadyAdded()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            var key = new CacheKey(1, 2);
            sut.AddOrReplace(key, "hello world");

            // Act
            var actual = sut.AddOrReplace(key, "hello world");

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void AddShouldReturnTrueWhenNewItemWasAdded()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            var key = new CacheKey(1, 2);

            // Act
            var actual = sut.AddOrReplace(key, "hello world");

            // Assert
            Assert.True(actual);
        }
    }
}