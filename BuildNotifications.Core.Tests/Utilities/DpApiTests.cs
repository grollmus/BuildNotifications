using BuildNotifications.PluginInterfaces;
using Xunit;

namespace BuildNotifications.Core.Tests.Utilities;

public class DpApiTests
{
    [Fact]
    public void EncryptingEmptyStringShouldNotBeEmptyString()
    {
        // Arrange
        const string plain = "";

        // Act
        var encrypted = DpApi.Encrypt(plain);

        // Assert
        Assert.NotEmpty(encrypted);
    }

    [Fact]
    public void MachineAndUserKeysAreDifferent()
    {
        // Arrange
        const string plain = "Hello World this is a test with some text";

        // Act
        var machine = DpApi.Encrypt(DpApi.KeyType.MachineKey, plain);
        var user = DpApi.Encrypt(DpApi.KeyType.UserKey, plain);

        // Assert
        Assert.NotEqual(machine, user);
    }

    [Fact]
    public void MachineLevelEncryptionCanBeDecrypted()
    {
        // Arrange
        const string plain = "Hello World this is a test with some text";

        // Act
        var encrypted = DpApi.Encrypt(plain);
        var decrypted = DpApi.Decrypt(encrypted);

        // Assert
        Assert.NotEqual(plain, encrypted);
        Assert.Equal(plain, decrypted);
    }

    [Fact]
    public void UserLevelEncryptionCanBeDecrypted()
    {
        // Arrange
        const string plain = "Hello World this is a test with some text";

        // Act
        var encrypted = DpApi.Encrypt(DpApi.KeyType.UserKey, plain);
        var decrypted = DpApi.Decrypt(encrypted);

        // Assert
        Assert.NotEqual(plain, encrypted);
        Assert.Equal(plain, decrypted);
    }
}