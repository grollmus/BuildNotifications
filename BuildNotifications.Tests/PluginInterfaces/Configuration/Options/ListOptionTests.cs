using System;
using System.Collections.Generic;
using System.Linq;
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

            public override IEnumerable<ListOptionItem<int>> AvailableValues => _filter(BaseAvailableValues);

            private IEnumerable<ListOptionItem<int>> BaseAvailableValues
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

            public void SetAvailableValuesFilter(Func<IEnumerable<ListOptionItem<int>>, IEnumerable<ListOptionItem<int>>> filter)
            {
                _filter = filter;
                RaiseAvailableValuesChanged();
            }

            private Func<IEnumerable<ListOptionItem<int>>, IEnumerable<ListOptionItem<int>>> _filter = x => x;
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

        [Fact]
        public void ValueShouldBeResettedWhenChangingAvailableValues()
        {
            // Arrange
            var sut = new TestListOption(3);

            // Act
            sut.SetAvailableValuesFilter(x => x.Take(1));

            // Assert
            var actual = sut.Value;
            Assert.Equal(1, actual);
        }
    }
}