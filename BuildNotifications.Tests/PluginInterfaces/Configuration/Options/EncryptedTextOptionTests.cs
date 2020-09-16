using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options
{
    public class EncryptedTextOptionTests
    {
        [Fact]
        public void ValueShouldBeEncrypted()
        {
            // Arrange
            var plainValue = "test";
            var sut = new EncryptedTextOption(plainValue, "name", "description");

            // Act
            var actual = sut.Value;

            // Assert
            Assert.NotNull(actual);
            var decrypted = actual.PlainText();
            Assert.Equal(plainValue, decrypted);
        }
    }
}