using System.Windows;
using System.Windows.Controls.Primitives;

namespace BuildNotifications.Resources.Icons;

internal class IconToggleButton : ToggleButton
{
    public IconType Icon
    {
        get => (IconType)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public double IconSize
    {
        get => (double)GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        "Icon", typeof(IconType), typeof(IconToggleButton), new PropertyMetadata(default(IconType)));

    public static readonly DependencyProperty IconSizeProperty = DependencyProperty.Register(
        "IconSize", typeof(double), typeof(IconToggleButton), new PropertyMetadata(default(double)));
}