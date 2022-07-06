using System.Linq;
using BuildNotifications.ViewModel.Settings.Options;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options;

public class EnumOptionViewModelTests
{
    private enum TestEnumWithoutLocalization
    {
        // ReSharper disable UnusedMember.Local
        NonLocalizableOne,
        NonLocalizableTwo,

        NonLocalizableThree
        // ReSharper enable UnusedMember.Local
    }

    [Fact]
    public void DisplayNameShouldUseToStringWhenNotLocalizable()
    {
        // Arrange
        var sut = new EnumOptionViewModel<TestEnumWithoutLocalization>("displayName");

        // Act
        var actual = sut.AvailableValues.First().DisplayName;

        // Assert
        Assert.Equal("NonLocalizableOne", actual);
    }
}