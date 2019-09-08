using System.Collections.Generic;
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
        public void ClearShouldClearCacheContent()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            sut.AddOrReplace(1, 1, "test");
            sut.AddOrReplace(1, 2, "test");
            sut.AddOrReplace(1, 3, "test");

            // Act
            sut.Clear();

            // Assert
            Assert.Empty(sut.ContentCopy());
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
        public void ContentCopyShouldReturnCopyOfStoredElements()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            sut.AddOrReplace(1, 1, "test1");
            sut.AddOrReplace(1, 2, "test2");
            sut.AddOrReplace(1, 3, "test3");

            // Act
            var actual = sut.ContentCopy();

            // Assert
            var expectedSubset = new HashSet<string> {"test1", "test2", "test3"};
            var actualSet = actual.ToHashSet();

            Assert.Subset(expectedSubset, actualSet);
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

        [Fact]
        public void RemoveValueShouldOnlyRemoveMatchingEntries()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            sut.AddOrReplace(1, 1, "test1");
            sut.AddOrReplace(1, 2, "test2");
            sut.AddOrReplace(1, 3, "test3");

            // Act
            sut.RemoveValue("test2");

            // Assert
            var expectedSubset = new HashSet<string> {"test1", "test3"};
            var actualSet = sut.ContentCopy().ToHashSet();

            Assert.Subset(expectedSubset, actualSet);
        }

        [Fact]
        public void ValuesShouldOnlyReturnMatchingValues()
        {
            // Arrange
            var sut = new PipelineCache<string>();
            sut.AddOrReplace(1, 1, "test");
            sut.AddOrReplace(2, 1, "wrong");
            sut.AddOrReplace(1, 2, "test");

            // Act
            var actual = sut.Values(1);

            // Assert
            Assert.All(actual, x => Assert.Equal("test", x));
        }
    }
}