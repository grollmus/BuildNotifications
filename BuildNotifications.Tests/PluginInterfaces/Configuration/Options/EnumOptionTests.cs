using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Tests.PluginInterfaces.Configuration.Options;

public class EnumOptionTests
{
    [Fact]
    public void AvailableValuesShouldContainAllEnumMembers()
    {
        // Arrange
        var sut = new EnumOption<TestEnum>(TestEnum.None, string.Empty, string.Empty);

        // Act
        var actual = sut.AvailableValues.ToList();

        // Assert
        Assert.Collection(actual,
            it => Assert.Equal(TestEnum.None, it.Value), it => Assert.Equal(TestEnum.One, it.Value),
            it => Assert.Equal(TestEnum.Two, it.Value),
            it => Assert.Equal(TestEnum.Three, it.Value));
    }

    [Fact]
    public void InitialValueShouldBeSet()
    {
        // Arrange
        var expected = TestEnum.Two;
        var sut = new EnumOption<TestEnum>(expected, string.Empty, string.Empty);

        // Act
        var acutal = sut.Value;

        // Assert
        Assert.Equal(expected, acutal);
    }

    [Fact]
    public void SettingValueShouldNotChangeCurrentValueWhenNewValueIsInvalid()
    {
        // Arrange
        var sut = new EnumOption<TestEnum>(TestEnum.None, string.Empty, string.Empty);

        // Act
        sut.Value = (TestEnum)12345;

        // Assert
        var actual = sut.Value;
        Assert.Equal(TestEnum.None, actual);
    }

    [Theory]
    [InlineData(TestEnum.One, true)]
    [InlineData(TestEnum.Three, true)]
    [InlineData((TestEnum)999, false)]
    public void SettingValueShouldOnlyAcceptEnumMembers(TestEnum value, bool accept)
    {
        // Arrange
        var sut = new EnumOption<TestEnum>(TestEnum.None, string.Empty, string.Empty);

        // Act
        sut.Value = value;

        // Assert
        if (accept)
            Assert.Equal(value, sut.Value);
        else
            Assert.NotEqual(value, sut.Value);
    }
}