using System;
using System.Globalization;
using BuildNotifications.Core.Text;
using BuildNotifications.ViewModel.Settings.Options;
using NSubstitute;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options;

public class OptionViewModelBaseTests
{
    private class TestOptionViewModel : OptionViewModelBase
    {
        public TestOptionViewModel()
            : base(string.Empty, string.Empty)
        {
        }

        public TestOptionViewModel(string name, string description, IStringLocalizer localizer)
            : base(name, description, localizer)
        {
            StringLocalizer = localizer;
        }

        public IStringLocalizer StringLocalizer { get; }
    }

    private class TestOptionViewModelInt : OptionViewModelBase<int>
    {
        public TestOptionViewModelInt()
            : base(0, string.Empty)
        {
        }
    }

    [Fact]
    public void DescriptionShouldBeLocalized()
    {
        // Arrange
        var localizer = Substitute.For<IStringLocalizer>();

        // Act
        var sut = new TestOptionViewModel("name", "description", localizer);

        // Assert
        sut.StringLocalizer.Received(1).GetText("description", Arg.Any<CultureInfo>());
    }

    [Fact]
    public void DisplayNameShouldBeLocalized()
    {
        // Arrange
        var localizer = Substitute.For<IStringLocalizer>();

        // Act
        var sut = new TestOptionViewModel("name", "description", localizer);

        // Assert
        sut.StringLocalizer.Received(1).GetText("name", Arg.Any<CultureInfo>());
    }

    [Fact]
    public void IsEnabledByDefault()
    {
        // Arrange
        var sut = new TestOptionViewModel();

        // Act
        var actual = sut.IsEnabled;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void IsNotLoadingByDefault()
    {
        // Arrange
        var sut = new TestOptionViewModel();

        // Act
        var actual = sut.IsLoading;

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void IsVisibleByDefault()
    {
        // Arrange
        var sut = new TestOptionViewModel();

        // Act
        var actual = sut.IsVisible;

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void SettingIsEnabledShouldRaiseEvent()
    {
        // Arrange
        var sut = new TestOptionViewModel();
        var tester = new NotifyPropertyChangedTester(sut);

        // Act
        sut.IsEnabled = !sut.IsEnabled;

        // Assert
        tester.AssertFiredOnly(nameof(OptionViewModelBase.IsEnabled));
    }

    [Fact]
    public void SettingIsLoadingShouldRaiseEvent()
    {
        // Arrange
        var sut = new TestOptionViewModel();
        var tester = new NotifyPropertyChangedTester(sut);

        // Act
        sut.IsLoading = !sut.IsLoading;

        // Assert
        tester.AssertFiredOnly(nameof(OptionViewModelBase.IsLoading));
    }

    [Fact]
    public void SettingIsVisibleShouldRaiseEvent()
    {
        // Arrange
        var sut = new TestOptionViewModel();
        var tester = new NotifyPropertyChangedTester(sut);

        // Act
        sut.IsVisible = !sut.IsVisible;

        // Assert
        tester.AssertFiredOnly(nameof(OptionViewModelBase.IsVisible));
    }

    [Fact]
    public void SettingValueShouldRaiseEvent()
    {
        // Arrange
        var sut = new TestOptionViewModelInt();

        // Act
        var evt = Assert.RaisesAny<EventArgs>(
            e => sut.ValueChanged += e,
            e => sut.ValueChanged -= e,
            () => sut.Value = sut.Value + 1);

        // Assert
        Assert.NotNull(evt);
        Assert.Same(sut, evt.Sender);
        Assert.Equal(EventArgs.Empty, evt.Arguments);
    }
}