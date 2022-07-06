using System;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace BuildNotifications.Core.Tests.Utilities;

public class PasswordStringConverterTests
{
    [Fact]
    public void ReadJsonShouldReturnEmptyPasswordStringWhenExceptionWasThrown()
    {
        // Arrange
        var sut = new PasswordStringConverter();
        var reader = Substitute.For<JsonReader>();
        reader.Value.Throws(new Exception());

        // Act
        var actual = sut.ReadJson(reader, typeof(PasswordString), new PasswordString(), false, JsonSerializer.CreateDefault());

        // Assert
        Assert.Empty(actual.PlainText());
    }

    [Fact]
    public void ReadJsonShouldReturnEmptyPasswordStringWhenValueIsEmpty()
    {
        // Arrange
        var sut = new PasswordStringConverter();
        var reader = Substitute.For<JsonReader>();
        const string expected = "";
        reader.Value.Returns(expected);

        // Act
        var actual = sut.ReadJson(reader, typeof(PasswordString), new PasswordString(), false, JsonSerializer.CreateDefault());

        // Assert
        Assert.Empty(actual.PlainText());
    }

    [Fact]
    public void ReadJsonShouldReturnValidPasswordString()
    {
        // Arrange
        var sut = new PasswordStringConverter();
        var reader = Substitute.For<JsonReader>();
        const string expected = "foobar";
        reader.Value.Returns(PasswordString.FromPlainText(expected).Encrypted());

        // Act
        var actual = sut.ReadJson(reader, typeof(PasswordString), new PasswordString(), false, JsonSerializer.CreateDefault());

        // Assert
        Assert.Equal(expected, actual.PlainText());
    }

    [Fact]
    public void WriteJsonShouldWriteEncryptedPassword()
    {
        // Arrange
        var sut = new PasswordStringConverter();
        var writer = Substitute.For<JsonWriter>();

        var plainText = "plain";
        var value = PasswordString.FromPlainText(plainText);

        // Act
        sut.WriteJson(writer, value, JsonSerializer.CreateDefault());

        // Assert
        writer.Received(1).WriteValue(Arg.Is<string>(arg => new PasswordString(arg).PlainText() == plainText));
    }
}