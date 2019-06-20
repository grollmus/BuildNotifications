using System.Collections;
using System.Collections.Generic;
using BuildNotifications.Core.Utilities;
using Xunit;

namespace BuildNotifications.Core.Tests.Utilities
{
    public class SerializerTests
    {
        private class EmptySerializerTestCases : IEnumerable<object[]>
        {
            /// <inheritdoc />
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new[] {new object()};
                yield return new object[] {123};
                yield return new object[] {"hello, world"};
                yield return new object[] {new int[0]};
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Fact]
        public void DeserializeShouldReturnSameDataWhenUsingSerializedOutput()
        {
            // Arrange
            var sut = new Serializer();
            const int expected = 123;
            var serialized = sut.Serialize(expected);

            // Act
            var actual = sut.Deserialize<int>(serialized);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [ClassData(typeof(EmptySerializerTestCases))]
        public void SerializeShouldNotBeEmptyWhenInputIsNotEmpty(object input)
        {
            // Arrange
            var sut = new Serializer();

            // Act
            var actual = sut.Serialize(input);

            // Assert
            Assert.NotEmpty(actual);
        }
    }
}