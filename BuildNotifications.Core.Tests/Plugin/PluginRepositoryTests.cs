using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using BuildNotifications.PluginInterfacesLegacy.Notification;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Plugin
{
    public class PluginRepositoryTests
    {
        [Fact]
        public void BuildShouldContainAllBuildPlugins()
        {
            // Arrange
            var expectedBuilds = new[]
            {
                Substitute.For<IBuildPlugin>(),
                Substitute.For<IBuildPlugin>(),
                Substitute.For<IBuildPlugin>()
            };

            var expectedSourceControl = new[]
            {
                Substitute.For<ISourceControlPlugin>(),
                Substitute.For<ISourceControlPlugin>(),
                Substitute.For<ISourceControlPlugin>()
            };

            var notificationProcessors = new INotificationProcessor[0];

            var typeMatcher = Substitute.For<ITypeMatcher>();

            // Act
            var sut = new PluginRepository(expectedBuilds, expectedSourceControl, notificationProcessors, typeMatcher);

            // Assert
            Assert.Collection(sut.Build,
                x => Assert.Same(expectedBuilds[0], x),
                x => Assert.Same(expectedBuilds[1], x),
                x => Assert.Same(expectedBuilds[2], x)
            );

            Assert.Collection(sut.SourceControl,
                x => Assert.Same(expectedSourceControl[0], x),
                x => Assert.Same(expectedSourceControl[1], x),
                x => Assert.Same(expectedSourceControl[2], x)
            );
        }

        [Fact]
        public void FindBuildPluginShouldFindPluginWhenExisting()
        {
            // Arrange
            var buildPlugins = new[]
            {
                Substitute.For<IBuildPlugin>()
            };
         
            var typeMatcher = Substitute.For<ITypeMatcher>();
            typeMatcher.MatchesType(buildPlugins[0].GetType(), "typeName").Returns(true);

            var sut = new PluginRepository(buildPlugins, new ISourceControlPlugin[0], new INotificationProcessor[0], typeMatcher);

            // Act
            var actual = sut.FindBuildPlugin("typeName");

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void FindBuildPluginShouldReturnNullWhenPluginWasNotFound()
        {
            // Arrange
            var sut = new PluginRepository(new IBuildPlugin[0], new ISourceControlPlugin[0], new INotificationProcessor[0], Substitute.For<ITypeMatcher>());

            // Act
            var actual = sut.FindBuildPlugin("non.existing");

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void FindSourceControlPluginShouldFindPluginWhenExisting()
        {
            // Arrange
            var sourceControlPlugins = new[]
            {
                Substitute.For<ISourceControlPlugin>()
            };

            var typeMatcher = Substitute.For<ITypeMatcher>();
            typeMatcher.MatchesType(sourceControlPlugins[0].GetType(), "typeName").Returns(true);

            var sut = new PluginRepository(new IBuildPlugin[0], sourceControlPlugins, new INotificationProcessor[0], typeMatcher);

            // Act
            var actual = sut.FindSourceControlPlugin("typeName");

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void FindSourceControlPluginShouldReturnNullWhenPluginWasNotFound()
        {
            // Arrange
            var sut = new PluginRepository(new IBuildPlugin[0], new ISourceControlPlugin[0], new INotificationProcessor[0], Substitute.For<ITypeMatcher>());

            // Act
            var actual = sut.FindSourceControlPlugin("non.existing");

            // Assert
            Assert.Null(actual);
        }
    }
}