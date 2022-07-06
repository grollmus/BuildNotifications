using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using BuildNotifications.PluginInterfaces.Builds;
using NLog.Fluent;

namespace BuildNotifications.Resources.Settings;

internal class PluginTypeToIconConverter : IValueConverter
{
    private PluginTypeToIconConverter()
    {
    }

    public static PluginTypeToIconConverter Instance { get; } = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IPlugin plugin)
        {
            try
            {
                return Geometry.Parse(plugin.IconSvgPath);
            }
            catch (Exception e)
            {
                Log.Debug().Message($"Failed to parse geometry data from plugin: {plugin.GetType()}").Exception(e).Write();
            }
        }

        return Geometry.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}