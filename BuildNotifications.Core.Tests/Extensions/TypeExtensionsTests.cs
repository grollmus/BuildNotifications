using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xunit;

namespace BuildNotifications.Core.Tests.Extensions
{
    public class TypeExtensionsTests
    {
        [Theory]
        [InlineData(typeof(List<string>), typeof(List<string>), true)]
        [InlineData(typeof(List<string>), typeof(List<int>), true)]
        [InlineData(typeof(List<string>), typeof(Collection<string>), false)]
        public void CompareWithoutGenericTypesShouldWorkForGenerics(Type typeA, Type typeB, bool expected)
        {
            // Act
            var actual = typeA.CompareWithoutGenericTypes(typeB);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(typeof(string), typeof(int), false)]
        [InlineData(typeof(byte), typeof(byte), true)]
        public void CompareWithoutGenericTypesShouldWorkForNonGenerics(Type typeA, Type typeB, bool expected)
        {
            // Act
            var actual = typeA.CompareWithoutGenericTypes(typeB);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData(typeof(List<string>), typeof(List<>), true)]
        [InlineData(typeof(List<string>), typeof(IEnumerable<>), true)]
        [InlineData(typeof(List<string>), typeof(Dictionary<,>), false)]
        public void IsAssignableToGenericTypeShouldWorkForGeneric(Type type, Type baseType, bool expected)
        {
            // Act
            var actual = type.IsAssignableToGenericType(baseType);

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}