using System;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Core.Tests.PluginInterfaces.Configuration.Options
{
    public class OptionTests
    {
        private class TestOption : Option
        {
            public TestOption()
                : base(string.Empty, string.Empty)
            {
            }

            public void SetIsLoading(bool isLoading)
            {
                IsLoading = isLoading;
            }
        }

        [Fact]
        public void IsEnabledByDefault()
        {
            // Arrange
            var sut = new TestOption();

            // Act
            var actual = sut.IsEnabled;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void IsNotLoadingByDefault()
        {
            // Arrange
            var sut = new TestOption();

            // Act
            var actual = sut.IsLoading;

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsVisibleByDefault()
        {
            // Arrange
            var sut = new TestOption();

            // Act
            var actual = sut.IsVisible;

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void SettingIsLoadingShouldRaiseEvent()
        {
            // Arrange
            var sut = new TestOption();

            // Act
            var evt = Assert.RaisesAny<EventArgs>(
                e => sut.IsLoadingChanged += e,
                e => sut.IsLoadingChanged -= e,
                () => sut.SetIsLoading(!sut.IsLoading));

            // Assert
            Assert.NotNull(evt);
            Assert.Same(sut, evt.Sender);
            Assert.Equal(EventArgs.Empty, evt.Arguments);
        }

        [Fact]
        public void SettingIsVisibleShouldShouldNotRaiseEventWhenIsLoadingIsNotChanged()
        {
            // Arrange
            var received = false;
            var sut = new TestOption();
            sut.IsLoadingChanged += (s, e) => received = true;

            // Act
            sut.SetIsLoading(sut.IsLoading);

            // Assert
            Assert.False(received);
        }

        [Fact]
        public void SettingIsVisibleShouldShouldNotRaiseEventWhenIsVisibleIsNotChanged()
        {
            // Arrange
            var received = false;
            var sut = new TestOption();
            sut.IsVisibleChanged += (s, e) => received = true;

            // Act
            sut.IsVisible = sut.IsVisible;

            // Assert
            Assert.False(received);
        }

        [Fact]
        public void SettingsIsVisibleShouldRaiseEvent()
        {
            // Arrange
            var sut = new TestOption();

            // Act
            ;
            var evt = Assert.RaisesAny<EventArgs>(
                e => sut.IsVisibleChanged += e,
                e => sut.IsVisibleChanged -= e,
                () => sut.IsVisible = !sut.IsVisible);

            // Assert
            Assert.NotNull(evt);
            Assert.Same(sut, evt.Sender);
            Assert.Equal(EventArgs.Empty, evt.Arguments);
        }
    }
}