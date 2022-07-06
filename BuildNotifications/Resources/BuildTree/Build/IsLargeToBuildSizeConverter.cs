using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace BuildNotifications.Resources.BuildTree.Build;

internal class IsLargeToBuildSizeConverter : IValueConverter
{
    private IsLargeToBuildSizeConverter()
    {
        _smallSize = (double)(Application.Current.FindResource("BlockOneAndHalf") ?? 2.5);
        _largeSize = (double)(Application.Current.FindResource("BlockTriple") ?? 10);
    }

    public static IsLargeToBuildSizeConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        var isLarge = value != null && (bool)value;
        return isLarge ? _largeSize : _smallSize;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    private readonly double _largeSize;
    private readonly double _smallSize;
}