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
        public void ContainsShouldDoNothingWhenItemIsNotContained()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            var key = new CacheKey(1, 2);

            // Act
            var ex = Record.Exception(() => sut.Remove(key));

            // Assert
            Assert.Null(ex);
        }

        [Fact]
        public void ContainsShouldReturnFalseWhenItemIsNotContained()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            var key = new CacheKey(1, 2);

            // Act
            var actual = sut.Contains(key);

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void ContainsShouldReturnTrueWhenItemIsContained()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            var key = new CacheKey(1, 2);
            sut.AddOrReplace(key, "hello world");

            // Act
            var actual = sut.Contains(key);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void RemoveShouldRemoveItemFromCache()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            var key = new CacheKey(1, 2);
            sut.AddOrReplace(key, "hello world");

            // Act
            var before = sut.Contains(key);
            sut.Remove(key);
            var after = sut.Contains(key);

            // Assert
            Assert.True(before);
            Assert.False(after);
        }
    }
}