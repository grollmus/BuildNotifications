using System;
using BuildNotifications.Plugin.Tfs.SourceControl;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Xunit;

namespace BuildNotifications.Tests.Plugins.Tfs.SourceControl;

public class TfsSourceControlProviderTests
{
    [Fact]
    public void ConstructorShouldNotThrowWhenNoConnectionIsPossible()
    {
        // Arrange
        var unknownHostName = $"https://host{Guid.NewGuid()}.com";
        var connection = new VssConnection(new Uri(unknownHostName), new VssCredentials());

        // Act
        var ex = Record.Exception(() => new TfsSourceControlProvider(connection, Guid.NewGuid(), Guid.NewGuid()));

        // Assert
        Assert.Null(ex);
    }
}