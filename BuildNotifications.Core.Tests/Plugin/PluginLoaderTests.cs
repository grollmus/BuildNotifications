using System.Linq;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Host;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Plugin
{
    public class PluginLoaderTests
    {
        [Fact]
        public void LoadPluginsShouldCreateEmptyRepositoryWhenNoFolderWasGiven()
        {
            // Arrange
            var sut = new PluginLoader(Substitute.For<IPluginHost>());

            // Act
            var actual = sut.LoadPlugins(Enumerable.Empty<string>());

            // Assert
            Assert.NotNull(actual);
            Assert.Empty(actual.Build);
            Assert.Empty(actual.SourceControl);
        }
    }
}