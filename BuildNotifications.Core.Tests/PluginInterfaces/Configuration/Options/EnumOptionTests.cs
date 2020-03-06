﻿using System;
using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using Xunit;

namespace BuildNotifications.Core.Tests.PluginInterfaces.Configuration.Options
{
    public class EnumOptionTests
    {
        public enum TestEnum
        {
            None,
            One,
            Two,
            Three
        }

        [Fact]
        public void AvailableValuesShouldContainAllEnumMembers()
        {
            // Arrange
            var sut = new EnumOption<TestEnum>(TestEnum.None, string.Empty, string.Empty);

            // Act
            var actual = sut.AvailableValues.ToList();

            // Assert
            var inspectors = new Action<ListOptionItem<TestEnum>>[]
            {
                it => Assert.Equal(TestEnum.None, it.Value),
                it => Assert.Equal(TestEnum.One, it.Value),
                it => Assert.Equal(TestEnum.Two, it.Value),
                it => Assert.Equal(TestEnum.Three, it.Value)
            };

            Assert.Collection(actual, inspectors);
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

        [Theory]
        [InlineData(TestEnum.One, true)]
        [InlineData(TestEnum.Three, true)]
        [InlineData((TestEnum) 999, false)]
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
}