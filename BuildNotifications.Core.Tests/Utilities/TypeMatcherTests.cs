using System;
using BuildNotifications.Core.Utilities;
using Xunit;

namespace BuildNotifications.Core.Tests.Utilities
{
    public class TypeMatcherTests
    {
        [Fact]
        public void AssemblyVersionShouldBeIgnored()
        {
            // Arrange
            var sut = new TypeMatcher();

            var type = typeof(string);
            var typeName = "System.String, System.Private.CoreLib, Version=1234.2.3.4";

            // Act
            var actual = sut.MatchesType(type, typeName);

            // Assert
            Assert.True(actual);
        }

        [Fact]
        public void PublicKeyTokenShouldBeIgnored()
        {
            // Arrange
            var sut = new TypeMatcher();

            var type = typeof(string);
            var typeName = @"System.String, System.Private.CoreLib, Version=4.0.0.0, PublicKeyToken=abcdef0123456789";

            // Act
            var actual = sut.MatchesType(type, typeName);

            // Assert
            Assert.True(actual);
        }

        [Theory]
        [InlineData(typeof(string), "System.String")]
        [InlineData(typeof(string), "System.string")]
        [InlineData(typeof(string), "SyStEM.StrING")]
        [InlineData(typeof(TypeMatcherTests), "BuildNotifications.Core.Tests.Utilities.TypeMatcherTests")]
        public void TypeShouldMatchWhenFullNameIsGiven(Type type, string typeName)
        {
            // Arrange
            var sut = new TypeMatcher();

            // Act
            var actual = sut.MatchesType(type, typeName);

            // Arrange
            Assert.True(actual);
        }

        [Theory]
        [InlineData(typeof(TypeMatcherTests), "Utilities.TypeMatcherTests")]
        [InlineData(typeof(TypeMatcherTests), "Tests.Utilities.TypeMatcherTests")]
        public void TypeShouldNotMatchWhenIncompleteNamespaceIsGiven(Type type, string typeName)
        {
            // Arrange
            var sut = new TypeMatcher();

            // Act
            var actual = sut.MatchesType(type, typeName);

            // Arrange
            Assert.False(actual);
        }

        [Theory]
        [InlineData(typeof(string), "string")]
        [InlineData(typeof(string), "String")]
        public void TypeShouldNotMatchWhenNoNamespaceIsGiven(Type type, string typeName)
        {
            // Arrange
            var sut = new TypeMatcher();

            // Act
            var actual = sut.MatchesType(type, typeName);

            // Arrange
            Assert.False(actual);
        }
    }
}