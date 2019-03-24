using BuildNotifications.Core.Config;
using BuildNotifications.Core.Utilities;
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
            var sut = new ConfigurationSerializer(serializer);

            // Act
            var config = sut.Load(fileName);

            // Assert
            Assert.NotNull(config);
        }
    }
}