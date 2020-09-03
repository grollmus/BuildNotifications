using System.Linq;
using BuildNotifications.ViewModel.Settings.Options;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options
{
    public class EnumOptionViewModelTests
    {
        private enum TestEnumWithoutLocalization
        {
            NonLocalizableOne,
            NonLocalizableTwo,
            NonLocalizableThree,
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
}