using System;
using System.Globalization;
using System.Windows.Data;
using BuildNotifications.Core.Text;
using NLog.Fluent;

namespace BuildNotifications.Resources.Text;

internal class EnumNameToLocalizedTextConverter : IValueConverter
{
    private EnumNameToLocalizedTextConverter()
    {
    }

    public static EnumNameToLocalizedTextConverter Instance { get; } = new();

    public object Convert(object? value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return "";

        try
        {
            var name = Enum.GetName(value.GetType(), value);
            if (name == null)
                return "";

            return StringLocalizer.Instance[name];
        }
        catch (Exception)
        {
            Log.Warn().Message("Failed to retrieve enum constant value from type: " + value.GetType()).Write();
            return "";
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}