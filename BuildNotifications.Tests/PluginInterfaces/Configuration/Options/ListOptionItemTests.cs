using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options;

public class ListOptionItemTests
{
    private class TestListOptionItem : ListOptionItem<int>
    {
        public TestListOptionItem(int value = 0, string displayName = null, bool localize = false)
            : base(value, displayName ?? string.Empty, localize)
        {
        }
    }

    [Fact]
    public void DisplayNameShouldBeUsedFromConstructor()
    {
        // Arrange
        const string displayName = "display-name";
        var sut = new TestListOptionItem(0, displayName);

        // Act
        var actualImplementation = sut.DisplayName;
        var actualInterface = ((IListOptionItem)sut).DisplayName;

        // Assert
        Assert.Equal(displayName, actualInterface);
        Assert.Equal(displayName, actualImplementation);
    }

    [Fact]
    public void ValueShouldBeUsedFromConstructor()
    {
        // Arrange
        const int value = 123;
        var sut = new TestListOptionItem(value);

        // Act
        var actualImplementation = sut.Value;
        var actualInterface = ((IListOptionItem)sut).Value;

        // Assert
        Assert.Equal(value, actualInterface);
        Assert.Equal(value, actualImplementation);
    }
}