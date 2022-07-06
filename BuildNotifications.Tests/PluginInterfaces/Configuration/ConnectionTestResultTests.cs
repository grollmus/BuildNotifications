using BuildNotifications.PluginInterfaces;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration;

public class ConnectionTestResultTests
{
    [Fact]
    public void FailureShouldContainMultipleMessages()
    {
        // Arrange
        var allMessages = new[]
        {
            "message-1",
            "error-2",
            "message-3"
        };
        var sut = ConnectionTestResult.Failure(allMessages);

        // Assert
        Assert.False(sut.IsSuccess);
        Assert.Equal(allMessages, sut.Errors);
        Assert.Contains(allMessages[0], sut.ErrorMessage);
        Assert.Contains(allMessages[1], sut.ErrorMessage);
        Assert.Contains(allMessages[2], sut.ErrorMessage);
    }

    [Fact]
    public void FailureShouldContainSingleMessage()
    {
        // Arrange
        const string message = "test-error";
        var sut = ConnectionTestResult.Failure(message);

        // Assert
        Assert.False(sut.IsSuccess);
        Assert.Contains(message, sut.ErrorMessage);
        Assert.Contains(message, sut.Errors);
    }

    [Fact]
    public void SuccessShouldNotContainAnyErrors()
    {
        // Arrange
        var sut = ConnectionTestResult.Success;

        // Assert
        Assert.True(sut.IsSuccess);
        Assert.Empty(sut.ErrorMessage);
        Assert.Empty(sut.Errors);
    }
}