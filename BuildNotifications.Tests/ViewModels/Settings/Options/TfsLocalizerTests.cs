using System.Globalization;
using BuildNotifications.Plugin.Tfs.Configuration;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options
{
    public class TfsLocalizerTests
    {
        [Fact]
        public void LocalizedShouldReturnLocalizedTextForTranslation()
        {
            // Arrange
            var sut = new TfsLocalizer();

            // Act
            var actual = sut.Localized("Password.Name", CultureInfo.CreateSpecificCulture("de"));

            // Assert
            Assert.Equal("Passwort", actual);
        }

        [Fact]
        public void LocalizedShouldReturnTextIdForMissingTranslation()
        {
            // Arrange
            var sut = new TfsLocalizer();

            // Act
            var actual = sut.Localized("MissingTranslationForTest", CultureInfo.CreateSpecificCulture("de"));

            // Assert
            Assert.Equal("[MissingTranslationForTest]", actual);
        }
    }
}