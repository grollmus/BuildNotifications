using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using BuildNotifications.ViewModel;
using BuildNotifications.ViewModel.Settings.Options;
using BuildNotifications.ViewModel.Settings.Options.PluginOptions;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options.PluginOptions
{
    public class PluginOptionViewModelImplementationTests
    {
        [Fact]
        public void DescriptionShouldBeLocalized()
        {
            // Arrange
            var localizationProvider = Substitute.For<ILocalizationProvider>();
            localizationProvider.Localize("desc").Returns("loc_desc");
            var option = Substitute.For<IOption>();
            option.DescriptionTextId.Returns("desc");
            var viewModel = Substitute.For<IViewModel>();
            var sut = new PluginOptionViewModelImplementation(option, localizationProvider, viewModel);

            // Act
            var actual = sut.Description;

            // Assert
            Assert.Equal("loc_desc", actual);
        }

        [Fact]
        public void DisplayNameShouldBeLocalized()
        {
            // Arrange
            var localizationProvider = Substitute.For<ILocalizationProvider>();
            localizationProvider.Localize("name").Returns("loc_name");
            var option = Substitute.For<IOption>();
            option.NameTextId.Returns("name");
            var viewModel = Substitute.For<IViewModel>();
            var sut = new PluginOptionViewModelImplementation(option, localizationProvider, viewModel);

            // Act
            var actual = sut.DisplayName;

            // Assert
            Assert.Equal("loc_name", actual);
        }

        [Fact]
        public void IsEnabledChangedShouldBeDispatched()
        {
            // Arrange
            var localizationProvider = Substitute.For<ILocalizationProvider>();
            var option = Substitute.For<IOption>();
            var viewModel = Substitute.For<IViewModel>();
            var unused = new PluginOptionViewModelImplementation(option, localizationProvider, viewModel);

            // Act
            option.IsEnabledChanged += Raise.Event();

            // Assert
            viewModel.Received(1).OnPropertyChanged(nameof(OptionViewModelBase.IsEnabled));
        }

        [Fact]
        public void IsLoadingChangedShouldBeDispatched()
        {
            // Arrange
            var localizationProvider = Substitute.For<ILocalizationProvider>();
            var option = Substitute.For<IOption>();
            var viewModel = Substitute.For<IViewModel>();
            var unused = new PluginOptionViewModelImplementation(option, localizationProvider, viewModel);

            // Act
            option.IsLoadingChanged += Raise.Event();

            // Assert
            viewModel.Received(1).OnPropertyChanged(nameof(OptionViewModelBase.IsLoading));
        }

        [Fact]
        public void IsVisibleChangedShouldBeDispatched()
        {
            // Arrange
            var localizationProvider = Substitute.For<ILocalizationProvider>();
            var option = Substitute.For<IOption>();
            var viewModel = Substitute.For<IViewModel>();
            var unused = new PluginOptionViewModelImplementation(option, localizationProvider, viewModel);

            // Act
            option.IsVisibleChanged += Raise.Event();

            // Assert
            viewModel.Received(1).OnPropertyChanged(nameof(OptionViewModelBase.IsVisible));
        }

        [Fact]
        public void ValueChangedShouldBeDispatched()
        {
            // Arrange
            var localizationProvider = Substitute.For<ILocalizationProvider>();
            var option = Substitute.For<ValueOption<int>>(0, string.Empty, string.Empty);
            var viewModel = Substitute.For<IViewModel>();
            var unused = new PluginOptionViewModelImplementation<int>(option, localizationProvider, viewModel);

            // Act
            option.ValueChanged += Raise.EventWith(new ValueChangedEventArgs<int>(0, 1));

            // Assert
            viewModel.Received(1).OnPropertyChanged(nameof(OptionViewModelBase<int>.Value));
        }
    }
}