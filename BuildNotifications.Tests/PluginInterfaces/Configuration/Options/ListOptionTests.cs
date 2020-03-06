using System;
using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options
{
    public class ListOptionTests
    {
        private class TestListOption : ListOption<int>
        {
            public TestListOption(int value)
                : base(value, string.Empty, string.Empty)
            {
            }

            public override IEnumerable<ListOptionItem<int>> AvailableValues
            {
                get
                {
                    yield return new ListOptionItem<int>(1, "1");
                    yield return new ListOptionItem<int>(2, "2");
                    yield return new ListOptionItem<int>(3, "3");
                }
            }

            public void FireAvailableValuesChangedEvent()
            {
                RaiseAvailableValuesChanged();
            }
        }

        [Fact]
        public void RaiseAvailableValuesChangedShouldRaiseEvent()
        {
            // Arrange
            var sut = new TestListOption(0);

            // Act
            var evt = Assert.RaisesAny<EventArgs>(
                e => sut.AvailableValuesChanged += e,
                e => sut.AvailableValuesChanged -= e,
                () => sut.FireAvailableValuesChangedEvent());

            // Assert
            Assert.NotNull(evt);
            Assert.Same(sut, evt.Sender);
            Assert.Equal(EventArgs.Empty, evt.Arguments);
        }

        [Theory]
        [InlineData(1, true)]
        [InlineData(3, true)]
        [InlineData(5, false)]
        public void SettingValueShouldOnlyAcceptValuesListedAsAvailable(int value, bool accept)
        {
            // Arrange
            var sut = new TestListOption(0);

            // Act
            sut.Value = value;

            // Assert
            if (accept)
                Assert.Equal(value, sut.Value);
            else
                Assert.NotEqual(value, sut.Value);
        }
    }
}