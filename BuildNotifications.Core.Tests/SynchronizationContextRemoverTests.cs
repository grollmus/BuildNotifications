using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BuildNotifications.Core.Tests;

public class SynchronizationContextRemoverTests
{
    [Fact]
    public async Task AwaitingShouldRemoveSynchronizationContext()
    {
        // Arrange
        var contextRemover = new SynchronizationContextRemover();

        // Act
        var contextBefore = SynchronizationContext.Current;
        await contextRemover;
        var contextAfter = SynchronizationContext.Current;

        // Assert
        Assert.NotNull(contextBefore);
        Assert.Null(contextAfter);
    }
}