using System;
using System.Globalization;
using System.Windows.Data;
using BuildNotifications.Core.Text;
using NLog.Fluent;

namespace BuildNotifications.Resources.Text;

internal class StringKeyToLocalizedTextConverter : IValueConverter
{
    public static StringKeyToLocalizedTextConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        var asString = value?.ToString();
        if (asString == null)
            return "";

        try
        {
            return StringLocalizer.Instance[asString];
        }
        catch (Exception)
        {
            Log.Warn().Message("Failed to retrieve localized text for key: " + asString).Write();
            return "";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}