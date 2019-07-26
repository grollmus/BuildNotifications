using System.Windows;
using System.Windows.Controls;

namespace BuildNotifications.Resources.Icons
{
    internal class IconButton : Button
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(IconType), typeof(IconButton), new PropertyMetadata(default(IconType)));

        public IconType Icon
        {
            get => (IconType) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(
            "IconSize", typeof(double), typeof(IconButton), new PropertyMetadata(default(double)));

        public double IconSize
        {
            get => (double) GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
    }
}
