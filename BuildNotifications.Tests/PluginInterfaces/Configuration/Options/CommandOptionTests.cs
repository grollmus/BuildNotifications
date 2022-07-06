using System;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options;

public class CommandOptionTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void CanExecuteShouldCallPredicate(bool expected)
    {
        // Arrange
        var sut = new CommandOption(() => Task.CompletedTask, () => expected, string.Empty, string.Empty);

        // Act
        var actual = sut.CanExecute();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task ExecuteShouldResetLoadingFlagWhenTaskThrowsException()
    {
        // Arrange
        var action = () => Task.FromException(new Exception("test"));
        var sut = new CommandOption(action, string.Empty, string.Empty);

        // Act
        var ex = await Record.ExceptionAsync(() => sut.Execute());

        // Assert
        Assert.NotNull(ex);
        Assert.False(sut.IsLoading);
    }

    [Fact]
    public async Task ExecuteShouldSetLoadingFlag()
    {
        // Arrange
        var action = () => Task.Delay(100);
        var sut = new CommandOption(action, string.Empty, string.Empty);

        // Act
        var before = sut.IsLoading;
        var task = sut.Execute();
        var during = sut.IsLoading;
        await task;
        var after = sut.IsLoading;

        // Assert
        Assert.False(before);
        Assert.True(during);
        Assert.False(after);
    }
}