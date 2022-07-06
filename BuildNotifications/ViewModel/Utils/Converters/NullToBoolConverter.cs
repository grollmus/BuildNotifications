using System;
using System.Globalization;
using System.Windows.Data;

namespace BuildNotifications.ViewModel.Utils.Converters;

internal class NullToBoolConverter : IValueConverter
{
    private NullToBoolConverter(bool invert)
    {
        _invert = invert;
    }

    public static NullToBoolConverter Instance { get; } = new(false);
    public static NullToBoolConverter Inverted { get; } = new(true);

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) => _invert ? value == null : value != null;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    private readonly bool _invert;
}