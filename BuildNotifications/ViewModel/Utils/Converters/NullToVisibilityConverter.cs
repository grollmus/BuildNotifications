using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BuildNotifications.ViewModel.Utils.Converters;

internal class NullToVisibilityConverter : IValueConverter
{
    private NullToVisibilityConverter(bool inverted)
    {
        _inverted = inverted;
    }

    public static NullToVisibilityConverter Instance { get; } = new(false);
    public static NullToVisibilityConverter Inverted { get; } = new(true);

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return _inverted ? Visibility.Visible : Visibility.Collapsed;

        return _inverted ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    private readonly bool _inverted;
}