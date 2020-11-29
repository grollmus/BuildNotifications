using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BuildNotifications.Resources.Global.Navigation.ButtonNavigation
{
    internal class ButtonNavigation : Control
    {
        public ICommand? AddItemCommand
        {
            get => (ICommand) GetValue(AddItemCommandProperty);
            set => SetValue(AddItemCommandProperty, value);
        }

        public bool DisplayAddMessage
        {
            get => (bool) GetValue(DisplayAddMessageProperty);
            private set => SetValue(DisplayAddMessagePropertyKey, value);
        }

        public IEnumerable<IButtonNavigationItem>? Items
        {
            get => (IEnumerable<IButtonNavigationItem>) GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public ICommand? RemoveItemCommand
        {
            get => (ICommand) GetValue(RemoveItemCommandProperty);
            set => SetValue(RemoveItemCommandProperty, value);
        }

        public IButtonNavigationItem SelectedItem
        {
            get => (IButtonNavigationItem) GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        private bool CalculateAddMessageVisibility()
        {
            if (AddItemCommand == null)
                return false;

            var items = Items;
            if (items == null)
                return false;

            return !items.Any();
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            UpdateAddMessage();
        }

        private void OnAddItemCommandChanged()
        {
            UpdateAddMessage();
        }

        private static void OnAddItemCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ButtonNavigation ctrl)
                ctrl.OnAddItemCommandChanged();
        }

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ButtonNavigation ctrl)
                ctrl.OnItemsChanged(e.OldValue as INotifyCollectionChanged);
        }

        private void OnItemsChanged(INotifyCollectionChanged? oldValue)
        {
            if (oldValue != null)
                oldValue.CollectionChanged -= Items_CollectionChanged;

            if (Items is INotifyCollectionChanged collectionChanged)
                collectionChanged.CollectionChanged += Items_CollectionChanged;

            UpdateAddMessage();
        }

        private void UpdateAddMessage()
        {
            DisplayAddMessage = CalculateAddMessageVisibility();
        }

        public static readonly DependencyProperty AddItemCommandProperty = DependencyProperty.Register(
            "AddItemCommand", typeof(ICommand), typeof(ButtonNavigation), new PropertyMetadata(default(ICommand), OnAddItemCommandChanged));

        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items", typeof(IEnumerable<IButtonNavigationItem>), typeof(ButtonNavigation), new PropertyMetadata(default(IEnumerable<IButtonNavigationItem>), OnItemsChanged));

        private static readonly DependencyPropertyKey DisplayAddMessagePropertyKey
            = DependencyProperty.RegisterReadOnly("DisplayAddMessage", typeof(bool), typeof(ButtonNavigation),
                new FrameworkPropertyMetadata(false,
                    FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty DisplayAddMessageProperty = DisplayAddMessagePropertyKey.DependencyProperty;

        public static readonly DependencyProperty RemoveItemCommandProperty = DependencyProperty.Register(
            "RemoveItemCommand", typeof(ICommand), typeof(ButtonNavigation), new PropertyMetadata(default(ICommand)));

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(IButtonNavigationItem), typeof(ButtonNavigation), new FrameworkPropertyMetadata(default(IButtonNavigationItem), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
    }
}