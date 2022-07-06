using System;
using System.Globalization;
using System.Windows.Data;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Resources.Settings;

internal class PluginTypeToDisplayNameConverter : IValueConverter
{
    private PluginTypeToDisplayNameConverter()
    {
    }

    public static PluginTypeToDisplayNameConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IPlugin plugin)
            return plugin.DisplayName;

        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}