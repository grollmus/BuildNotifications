using System;
using BuildNotifications.Plugin.Tfs.Build;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Xunit;

namespace BuildNotifications.Tests.Plugins.Tfs.Build;

public class TfsBuildProviderTests
{
    [Fact]
    public void ConstructorShouldNotThrowWhenNoConnectionIsPossible()
    {
        // Arrange
        var unknownHostName = $"https://host{Guid.NewGuid()}.com";
        var connection = new VssConnection(new Uri(unknownHostName), new VssCredentials());

        // Act
        var ex = Record.Exception(() => new TfsBuildProvider(connection, Guid.NewGuid()));

        // Assert
        Assert.Null(ex);
    }
}