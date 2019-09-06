using System.Linq;
using BuildNotifications.Core.Plugin;
using Xunit;

namespace BuildNotifications.Core.Tests.Plugin
{
    public class PluginLoaderTests
    {
        [Fact]
        public void LoadPluginsShouldCreateEmptyRepositoryWhenNoFolderWasGiven()
        {
            // Arrange
            var sut = new PluginLoader();

            // Act
            var actual = sut.LoadPlugins(Enumerable.Empty<string>());

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual.Build);
            Assert.Empty(actual.SourceControl);
        }
    }
}