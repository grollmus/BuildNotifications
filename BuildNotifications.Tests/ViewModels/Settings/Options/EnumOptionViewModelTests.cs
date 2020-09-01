using System.Linq;
using BuildNotifications.Tests.PluginInterfaces.Configuration.Options;
using BuildNotifications.ViewModel.Settings.Options;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options
{
    public class EnumOptionViewModelTests
    {
        [Fact]
        public void DisplayNameShouldUseToStringWhenNotLocalizable()
        {
            // Arrange
            var sut = new EnumOptionViewModel<TestEnum>("displayName");

            // Act
            var actual = sut.AvailableValues.First().DisplayName;

            // Assert
            Assert.Equal("None", actual);
        }
    }
}