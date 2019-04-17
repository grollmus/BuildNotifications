using System;
using System.Linq;
using BuildNotifications.Core.Plugin.Options;
using BuildNotifications.PluginInterfaces.Options;
using Xunit;

namespace BuildNotifications.Core.Tests.Plugin.Options
{
    public class OptionSchemaFactoryTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void FlagShouldHaveCorrectDefaultValue(bool value)
        {
            // Arrange
            var sut = new OptionSchemaFactory();

            // Act
            var option = sut.Flag("name", "description", value);

            // Assert
            Assert.Equal(value, option.DefaultValue);
        }

        [Fact]
        public void FlagShouldHaveCorrectDescription()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.Flag("name", expected);

            // Assert
            Assert.Equal(expected, option.Description);
        }

        [Fact]
        public void FlagShouldHaveCorrectName()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.Flag(expected, null);

            // Assert
            Assert.Equal(expected, option.Name);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void FlagShouldHaveCorrectRequiredValue(bool value)
        {
            // Arrange
            var sut = new OptionSchemaFactory();

            // Act
            var option = sut.Flag("name", "description", false, value);

            // Assert
            Assert.Equal(value, option.Required);
        }

        [Fact]
        public void GroupShouldContainCorrectTitle()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "title";

            // Act
            var group = sut.Group(expected);

            // Assert
            Assert.Equal(expected, group.Title);
        }

        [Fact]
        public void NumberShouldHaveCorrectDefaultValue()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const int expected = 123;

            // Act
            var option = sut.Number("name", "description", null, null, expected);

            // Assert
            Assert.Equal(expected, option.DefaultValue);
        }

        [Fact]
        public void NumberShouldHaveCorrectDescription()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.Number("name", expected);

            // Assert
            Assert.Equal(expected, option.Description);
        }

        [Fact]
        public void NumberShouldHaveCorrectMaximum()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const int expected = 123;

            // Act
            var option = sut.Number("name", "description", null, expected);

            // Assert
            Assert.Equal(expected, option.MaxValue);
        }

        [Fact]
        public void NumberShouldHaveCorrectMinimum()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const int expected = 123;

            // Act
            var option = sut.Number("name", "description", expected);

            // Assert
            Assert.Equal(expected, option.MinValue);
        }

        [Fact]
        public void NumberShouldHaveCorrectName()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.Number(expected, null);

            // Assert
            Assert.Equal(expected, option.Name);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void NumberShouldHaveCorrectRequiredValue(bool value)
        {
            // Arrange
            var sut = new OptionSchemaFactory();

            // Act
            var option = sut.Number("name", "description", null, null, 0, value);

            // Assert
            Assert.Equal(value, option.Required);
        }

        [Fact]
        public void NumberShouldThrowWhenMinimumIsBiggerThanMaximum()
        {
            // Arrange
            var sut = new OptionSchemaFactory();

            // Act
            var ex = Record.Exception(() => sut.Number("name", "desc", 2, 1));

            // Assert
            Assert.IsType<ArgumentException>(ex);
        }

        [Fact]
        public void SchemaShouldNotBeNull()
        {
            // Arrange
            var sut = new OptionSchemaFactory();

            // Act
            var actual = sut.Schema();

            // Assert
            Assert.NotNull(actual);
        }

        [Fact]
        public void SetItemShouldHaveCorrectDescription()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.SetItem("name", expected, null);

            // Assert
            Assert.Equal(expected, option.Description);
        }

        [Fact]
        public void SetItemShouldHaveCorrectName()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.SetItem(expected, "desc", null);

            // Assert
            Assert.Equal(expected, option.Name);
        }

        [Fact]
        public void SetItemShouldHaveCorrectValue()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const int expected = 123;

            // Act
            var option = sut.SetItem("name", "desc", expected);

            // Assert
            Assert.Equal(expected, option.Value);
        }

        [Fact]
        public void SetShouldHaveCorrectDescription()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.Set("name", expected, Enumerable.Empty<ISetItem>());

            // Assert
            Assert.Equal(expected, option.Description);
        }

        [Fact]
        public void SetShouldHaveCorrectName()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.Set(expected, null, Enumerable.Empty<ISetItem>());

            // Assert
            Assert.Equal(expected, option.Name);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SetShouldHaveCorrectRequiredValue(bool value)
        {
            // Arrange
            var sut = new OptionSchemaFactory();

            // Act
            var option = sut.Set("name", "description", Enumerable.Empty<ISetItem>(), null, value);

            // Assert
            Assert.Equal(value, option.Required);
        }

        [Fact]
        public void TextShouldHaveCorrectDefaultValue()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.Text("name", "description", expected);

            // Assert
            Assert.Equal(expected, option.DefaultValue);
        }

        [Fact]
        public void TextShouldHaveCorrectDescription()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.Text("name", expected);

            // Assert
            Assert.Equal(expected, option.Description);
        }

        [Fact]
        public void TextShouldHaveCorrectName()
        {
            // Arrange
            var sut = new OptionSchemaFactory();
            const string expected = "expected";

            // Act
            var option = sut.Text(expected, null);

            // Assert
            Assert.Equal(expected, option.Name);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TextShouldHaveCorrectRequiredValue(bool value)
        {
            // Arrange
            var sut = new OptionSchemaFactory();

            // Act
            var option = sut.Text("name", "description", null, value);

            // Assert
            Assert.Equal(value, option.Required);
        }
    }
}