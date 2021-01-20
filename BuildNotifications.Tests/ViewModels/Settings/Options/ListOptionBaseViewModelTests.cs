using System.Collections.Generic;
using System.Linq;
using BuildNotifications.ViewModel.Settings.Options;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options
{
    public class ListOptionBaseViewModelTests
    {
        private enum TestEnum
        {
            None,
            One,
            Two
        }

        private class TestListOptionViewModel : ListOptionBaseViewModel<TestEnum>
        {
            public TestListOptionViewModel()
                : base(string.Empty)
            {
            }

            protected override IEnumerable<TestEnum> ModelValues
            {
                get
                {
                    yield return TestEnum.None;
                    yield return TestEnum.One;
                    yield return TestEnum.Two;
                }
            }
        }

        [Fact]
        public void AvailableValuesShouldContainAllModelValues()
        {
            // Arrange
            var sut = new TestListOptionViewModel();

            // Act
            var actual = sut.AvailableValues.ToList();

            // Assert
            Assert.Collection(actual,
                a => Assert.Equal(TestEnum.None, a.Value),
                a => Assert.Equal(TestEnum.One, a.Value),
                a => Assert.Equal(TestEnum.Two, a.Value)
            );
        }
    }
}