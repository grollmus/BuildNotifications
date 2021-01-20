using System.Globalization;
using BuildNotifications.Core.Config;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Core.Tests.Config
{
    public class ConfigurationBuilderTests
    {
        [Fact]
        public void EmptyConfigurationShouldHaveCorrectName()
        {
            // Arrange
            var pathResolver = Substitute.For<IPathResolver>();
            var configurationSerializer = Substitute.For<IConfigurationSerializer>();
            var sut = new ConfigurationBuilder(pathResolver, configurationSerializer);

            var name = "test_name";

            // Act
            var actual = sut.EmptyConfiguration(name);

            // Assert
            Assert.Equal(name, actual.ProjectName);
        }

        [Fact]
        public void LoadConfigurationShouldApplyLoadedCulture()
        {
            // Arrange
            var pathResolver = Substitute.For<IPathResolver>();
            pathResolver.UserConfigurationFilePath.Returns("user.config");

            var configurationSerializer = Substitute.For<IConfigurationSerializer>();
            configurationSerializer.Load("user.config", out _).Returns(new Configuration {Language = "es"});

            var sut = new ConfigurationBuilder(pathResolver, configurationSerializer);

            var resetCulture = CultureInfo.CurrentUICulture;
            try
            {
                // Act
                var loaded = sut.LoadConfiguration();

                // Assert
                Assert.NotNull(loaded);
                Assert.Equal("es", CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            }
            finally
            {
                CultureInfo.CurrentUICulture = resetCulture;
            }
        }
    }
}