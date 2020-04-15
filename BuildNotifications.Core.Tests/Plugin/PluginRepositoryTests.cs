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

            var notificationProcessors = System.Array.Empty<INotificationProcessor>();

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

            var sut = new PluginRepository(buildPlugins, System.Array.Empty<ISourceControlPlugin>(), System.Array.Empty<INotificationProcessor>(), typeMatcher);

            // Act
            var actual = sut.FindBuildPlugin("typeName");

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void FindBuildPluginShouldReturnNullWhenPluginWasNotFound()
        {
            // Arrange
            var sut = new PluginRepository(System.Array.Empty<IBuildPlugin>(), System.Array.Empty<ISourceControlPlugin>(), System.Array.Empty<INotificationProcessor>(), Substitute.For<ITypeMatcher>());

            // Act
            var actual = sut.FindBuildPlugin("non.existing");

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void FindConfigurationTypeShouldReturnNameOfMatchedBuildPlugin()
        {
            // Arrange
            var buildPlugin = Substitute.For<IBuildPlugin>();
            var expected = typeof(string);
            buildPlugin.GetConfigurationType().Returns(expected);

            var buildPlugins = new[]
            {
                buildPlugin
            };

            var typeMatcher = Substitute.For<ITypeMatcher>();
            typeMatcher.MatchesType(buildPlugins[0].GetType(), "typeName").Returns(true);

            var sut = new PluginRepository(buildPlugins, System.Array.Empty<ISourceControlPlugin>(), System.Array.Empty<INotificationProcessor>(), typeMatcher);

            // Act
            var actual = sut.FindConfigurationType("typeName");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FindConfigurationTypeShouldReturnNullWhenPluginWasNotFound()
        {
            // Arrange
            var sut = new PluginRepository(System.Array.Empty<IBuildPlugin>(), System.Array.Empty<ISourceControlPlugin>(), System.Array.Empty<INotificationProcessor>(), new TypeMatcher());

            // Act
            var actual = sut.FindConfigurationType("typeName");

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void FindIconGeometryShouldReturnNameOfMatchedBuildPlugin()
        {
            // Arrange
            var buildPlugin = Substitute.For<IBuildPlugin>();
            const string expected = "hello world";
            buildPlugin.IconSvgPath.Returns(expected);

            var buildPlugins = new[]
            {
                buildPlugin
            };

            var typeMatcher = Substitute.For<ITypeMatcher>();
            typeMatcher.MatchesType(buildPlugins[0].GetType(), "typeName").Returns(true);

            var sut = new PluginRepository(buildPlugins, System.Array.Empty<ISourceControlPlugin>(), System.Array.Empty<INotificationProcessor>(), typeMatcher);

            // Act
            var actual = sut.FindIconGeometry("typeName");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FindIconGeometryShouldReturnNullWhenPluginWasNotFound()
        {
            // Arrange
            var sut = new PluginRepository(System.Array.Empty<IBuildPlugin>(), System.Array.Empty<ISourceControlPlugin>(), System.Array.Empty<INotificationProcessor>(), new TypeMatcher());

            // Act
            var actual = sut.FindIconGeometry("typeName");

            // Assert
            Assert.Null(actual);
        }

        [Fact]
        public void FindPluginNameShouldReturnNameOfMatchedBuildPlugin()
        {
            // Arrange
            var buildPlugin = Substitute.For<IBuildPlugin>();
            const string expected = "hello world";
            buildPlugin.DisplayName.Returns(expected);

            var buildPlugins = new[]
            {
                buildPlugin
            };

            var typeMatcher = Substitute.For<ITypeMatcher>();
            typeMatcher.MatchesType(buildPlugins[0].GetType(), "typeName").Returns(true);

            var sut = new PluginRepository(buildPlugins, System.Array.Empty<ISourceControlPlugin>(), System.Array.Empty<INotificationProcessor>(), typeMatcher);

            // Act
            var actual = sut.FindPluginName("typeName");

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void FindPluginNameShouldReturnNullWhenPluginWasNotFound()
        {
            // Arrange
            var sut = new PluginRepository(System.Array.Empty<IBuildPlugin>(), System.Array.Empty<ISourceControlPlugin>(), System.Array.Empty<INotificationProcessor>(), new TypeMatcher());

            // Act
            var actual = sut.FindPluginName("typeName");

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

            var sut = new PluginRepository(System.Array.Empty<IBuildPlugin>(), sourceControlPlugins, System.Array.Empty<INotificationProcessor>(), typeMatcher);

            // Act
            var actual = sut.FindSourceControlPlugin("typeName");

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void FindSourceControlPluginShouldReturnNullWhenPluginWasNotFound()
        {
            // Arrange
            var sut = new PluginRepository(System.Array.Empty<IBuildPlugin>(), System.Array.Empty<ISourceControlPlugin>(), System.Array.Empty<INotificationProcessor>(), Substitute.For<ITypeMatcher>());

            // Act
            var actual = sut.FindSourceControlPlugin("non.existing");

            // Assert
            Assert.Null(actual);
        }
    }
}