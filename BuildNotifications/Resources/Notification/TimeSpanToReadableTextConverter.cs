using System;
using System.Globalization;
using System.Windows.Data;
using BuildNotifications.Core.Utilities;

namespace BuildNotifications.Resources.Notification;

internal class TimeSpanToReadableTextConverter : IValueConverter
{
    private TimeSpanToReadableTextConverter()
    {
        _converter = new TimeSpanToStringConverter();
    }

    public static TimeSpanToReadableTextConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not TimeSpan timeSpan)
            return "";

        return _converter.Convert(timeSpan);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();

    private readonly TimeSpanToStringConverter _converter;
}