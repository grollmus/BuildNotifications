using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BuildNotifications.Resources.Animation.Checkbox;

internal class CheckboxSelectedToMarginConverter : IValueConverter
{
    static CheckboxSelectedToMarginConverter()
    {
        BlockSize = (double)Application.Current.TryFindResource("Block");
    }

    public static Thickness CheckedMargin => new(BlockSize * 2.0, 0, 0, 0);

    public static CheckboxSelectedToMarginConverter Instance { get; } = new();

    public static Thickness UncheckedMargin => new(BlockSize / 6.0, 0, 0, 0);
    private static double BlockSize { get; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool asBool)
            return new Thickness(0);

        return asBool ? CheckedMargin : UncheckedMargin;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}