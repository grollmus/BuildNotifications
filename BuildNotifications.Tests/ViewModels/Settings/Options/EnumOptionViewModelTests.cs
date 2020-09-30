using System.Linq;
using BuildNotifications.ViewModel.Settings.Options;
using JetBrains.Annotations;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options
{
    public class EnumOptionViewModelTests
    {
        private enum TestEnumWithoutLocalization
        {
            [UsedImplicitly] NonLocalizableOne,
            [UsedImplicitly] NonLocalizableTwo,
            [UsedImplicitly] NonLocalizableThree,
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