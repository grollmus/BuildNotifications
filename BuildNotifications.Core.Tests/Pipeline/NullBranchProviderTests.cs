using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Pipeline;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline;

public class NullBranchProviderTests
{
    [Fact]
    public void BranchNameExtractorShouldNotBeNull()
    {
        // Arrange
        var sut = new NullBranchProvider();

        // Act
        var actual = sut.NameExtractor;

        // Assert
        Assert.NotNull(actual);
    }

    [Fact]
    public async Task FetchExistingBranchesShouldReturnEmptyList()
    {
        // Arrange
        var sut = new NullBranchProvider();

        // Act
        var actual = await sut.FetchExistingBranches().ToListAsync();

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public async Task RemovedBranchesShouldReturnEmptyList()
    {
        // Arrange
        var sut = new NullBranchProvider();

        // Act
        var actual = await sut.RemovedBranches().ToListAsync();

        // Assert
        Assert.Empty(actual);
    }
}