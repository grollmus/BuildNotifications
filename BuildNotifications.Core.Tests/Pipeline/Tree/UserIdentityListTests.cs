using BuildNotifications.Core.Pipeline;
using Xunit;

namespace BuildNotifications.Core.Tests.Pipeline.Tree
{
    public class UserIdentityListTests
    {
        [Fact]
        public void ConstructorShouldSetCorrectValues()
        {
            // Arrange

            // Act
            var sut = new UserIdentityList();

            // Assert
            Assert.NotNull(sut.IdentitiesOfCurrentUser);
            Assert.Empty(sut.IdentitiesOfCurrentUser);
        }
    }
}