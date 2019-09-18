using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.Resources.Menu
{
    internal class DecoratedMenuItem : MenuItem
    {
        public IconType IconType
        {
            get => (IconType) GetValue(IconTypeProperty);
            set => SetValue(IconTypeProperty, value);
        }

        public static readonly DependencyProperty IconTypeProperty = DependencyProperty.Register(
            "IconType", typeof(IconType), typeof(DecoratedMenuItem), new PropertyMetadata(default(IconType)));
    }
}