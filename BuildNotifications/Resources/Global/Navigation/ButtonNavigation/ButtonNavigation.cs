using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BuildNotifications.Resources.Global.Navigation.ButtonNavigation
{
    internal class ButtonNavigation : Control
    {
        public static readonly DependencyProperty ItemsProperty = DependencyProperty.Register(
            "Items", typeof(ObservableCollection<IButtonNavigationItem>), typeof(ButtonNavigation), new PropertyMetadata(default(ObservableCollection<IButtonNavigationItem>)));

        public ObservableCollection<IButtonNavigationItem> Items
        {
            get => (ObservableCollection<IButtonNavigationItem>) GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem", typeof(IButtonNavigationItem), typeof(ButtonNavigation), new PropertyMetadata(default(IButtonNavigationItem)));

        public IButtonNavigationItem SelectedItem
        {
            get => (IButtonNavigationItem) GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        public ButtonNavigation()
        {
            Items = new ObservableCollection<IButtonNavigationItem>();
        }
    }
}
