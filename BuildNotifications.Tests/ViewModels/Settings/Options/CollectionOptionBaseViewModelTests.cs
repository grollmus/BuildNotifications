using BuildNotifications.ViewModel.Settings.Options;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options
{
    public class CollectionOptionBaseViewModelTests
    {
        private class CollectionOptionViewModel : CollectionOptionBaseViewModel<int, NumberOptionViewModel>
        {
            public CollectionOptionViewModel()
                : base(new int[0], string.Empty)
            {
            }

            protected override NumberOptionViewModel CreateNewValue() => CreateNewValue(0);

            protected override NumberOptionViewModel CreateNewValue(int value) => new NumberOptionViewModel(value, 0, 10, string.Empty);
        }

        [Fact]
        public void AddNewItemCommandShouldAddNewItem()
        {
            // Arrange
            var sut = new CollectionOptionViewModel();

            // Act
            sut.AddNewItemCommand.Execute(null);

            // Assert
            Assert.Single(sut.Values);
        }

        [Fact]
        public void RemoveItemCommandShouldDoNothingWhenValueIsNotFound()
        {
            // Arrange
            var sut = new CollectionOptionViewModel();

            // Act
            sut.RemoveItemCommand.Execute(123);

            // Assert
            Assert.Empty(sut.Values);
        }

        [Fact]
        public void RemoveItemCommandShouldRemoveItem()
        {
            // Arrange
            var sut = new CollectionOptionViewModel();
            sut.Values.Add(new NumberOptionViewModel(123, 0, 1000, string.Empty));

            // Act
            sut.RemoveItemCommand.Execute(123);

            // Assert
            Assert.Empty(sut.Values);
        }
    }
}