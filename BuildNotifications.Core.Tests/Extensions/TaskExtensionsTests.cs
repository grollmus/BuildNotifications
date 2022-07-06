using System.Threading.Tasks;
using Xunit;

namespace BuildNotifications.Core.Tests.Extensions;

public class TaskExtensionsTests
{
    [Fact]
    public void IgnoreResultShouldWorkOnTask()
    {
        // Arrange
        var task = Task.Delay(1);

        // Act
        var ex = Record.Exception(() => task.IgnoreResult());

        // Assert
        Assert.Null(ex);
    }
}