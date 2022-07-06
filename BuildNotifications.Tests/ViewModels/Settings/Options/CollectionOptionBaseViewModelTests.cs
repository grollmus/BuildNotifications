using System;
using BuildNotifications.ViewModel.Settings.Options;
using Xunit;

namespace BuildNotifications.Tests.ViewModels.Settings.Options;

public class CollectionOptionBaseViewModelTests
{
    private class CollectionOptionViewModel : CollectionOptionBaseViewModel<int, NumberOptionViewModel>
    {
        public CollectionOptionViewModel()
            : base(Array.Empty<int>(), string.Empty)
        {
        }

        protected override NumberOptionViewModel CreateNewValue() => CreateNewValue(0);

        protected override NumberOptionViewModel CreateNewValue(int value) => new(value, 0, 10, string.Empty);
    }

    private class CollectionOptionViewModelWithDisabledAdd : CollectionOptionViewModel
    {
        protected override bool CanAddNewItem() => false;
    }

    private class CollectionOptionViewModelWithDisabledRemove : CollectionOptionViewModel
    {
        protected override bool CanRemoveItem(int value) => false;
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
    public void AddNewItemCommandShouldBeDisabledWhenCanAddReturnsFalse()
    {
        // Arrange
        var sut = new CollectionOptionViewModelWithDisabledAdd();

        // Act
        var actual = sut.AddNewItemCommand.CanExecute(null);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void AddNewItemCommandShouldBeEnabledWhenNotOverriden()
    {
        // Arrange
        var sut = new CollectionOptionViewModel();

        // Act
        var actual = sut.AddNewItemCommand.CanExecute(null);

        // Assert
        Assert.True(actual);
    }

    [Fact]
    public void RemoveItemCommandShouldBeDisabledWhenCanRemoveReturnsFalse()
    {
        // Arrange
        var sut = new CollectionOptionViewModelWithDisabledRemove();

        // Act
        var actual = sut.RemoveItemCommand.CanExecute(0);

        // Assert
        Assert.False(actual);
    }

    [Fact]
    public void RemoveItemCommandShouldBeEnabledWhenNotOverriden()
    {
        // Arrange
        var sut = new CollectionOptionViewModel();

        // Act
        var actual = sut.RemoveItemCommand.CanExecute(0);

        // Assert
        Assert.True(actual);
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