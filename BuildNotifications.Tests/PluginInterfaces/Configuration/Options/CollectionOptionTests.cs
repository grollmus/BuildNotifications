using System;
using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options;

public class CollectionOptionTests
{
    private class TestCollectionOption : CollectionOption<int>
    {
        public TestCollectionOption(IEnumerable<int> value)
            : base(value, string.Empty, string.Empty)
        {
        }
    }

    [Fact]
    public void AddItemShouldRaiseChangedEvent()
    {
        // Arrange
        var sut = new TestCollectionOption(Array.Empty<int>());

        // Act
        var evt = Assert.Raises<EventArgs>(
            e => sut.ValueChanged += e,
            e => sut.ValueChanged -= e,
            () => sut.AddNewItem(123));

        // Assert
        Assert.NotNull(evt);
        Assert.Same(sut, evt.Sender);
    }

    [Fact]
    public void RemoveItemShouldRaiseChangedEvent()
    {
        // Arrange
        var sut = new TestCollectionOption(Array.Empty<int>());

        // Act
        var evt = Assert.Raises<EventArgs>(
            e => sut.ValueChanged += e,
            e => sut.ValueChanged -= e,
            () => sut.RemoveItem(123));

        // Assert
        Assert.NotNull(evt);
        Assert.Same(sut, evt.Sender);
    }
}