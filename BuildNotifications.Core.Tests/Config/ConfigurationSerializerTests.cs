using System.Collections.Generic;
using System.IO;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces.Builds;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Config
{
    public class ConfigurationSerializerTests
    {
        [Fact]
        public void LoadShouldNotCrashWhenFileDoesNotExist()
        {
            // Arrange
            const string fileName = "non.existing";
            var serializer = Substitute.For<ISerializer>();
            var pluginRepo = Substitute.For<IPluginRepository>();

            if (File.Exists(fileName))
                File.Delete(fileName);
            var sut = new ConfigurationSerializer(serializer, pluginRepo);

            // Act
            var config = sut.Load(fileName);

            // Assert
            Assert.NotNull(config);
        }

        [Fact]
        public void LoadShouldNotCreateFileWhenFileDoesNotExist()
        {
            // Arrange
            const string fileName = "non.existing";
            var serializer = Substitute.For<ISerializer>();
            var pluginRepo = Substitute.For<IPluginRepository>();

            if (File.Exists(fileName))
                File.Delete(fileName);

            var sut = new ConfigurationSerializer(serializer, pluginRepo);

            // Act
            var unused = sut.Load(fileName);

            // Assert
            Assert.False(File.Exists(fileName));
        }

        [Fact]
        public void LoadShouldSetBuildAndSourceControlFunctionsOfPluginRepo()
        {
            // Arrange
            const string fileName = "non.existing";
            var serializer = Substitute.For<ISerializer>();
            var pluginRepo = Substitute.For<IPluginRepository>();
            pluginRepo.Build.Returns(new List<IBuildPlugin> {Substitute.For<IBuildPlugin>()});

            if (File.Exists(fileName))
                File.Delete(fileName);

            var sut = new ConfigurationSerializer(serializer, pluginRepo);

            // Act
            var config = sut.Load(fileName);

            // Assert
            Assert.True(((Configuration) config).PossibleBuildPlugins().Any());
        }
    }
}