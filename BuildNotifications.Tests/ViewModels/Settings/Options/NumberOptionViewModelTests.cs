using BuildNotifications.ViewModel.Settings.Options;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options;

public class NumberOptionViewModelTests
{
    [Fact]
    public void ConstructorShouldSetCorrectMinMaxValues()
    {
        // Arrange
        const int expectedMin = 5;
        const int expectedMax = 12;

        // Act
        var actual = new NumberOptionViewModel(10, expectedMin, expectedMax, string.Empty);

        // Assert
        Assert.Equal(expectedMin, actual.MinValue);
        Assert.Equal(expectedMax, actual.MaxValue);
    }
}