using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Anotar.NLog;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Resources.Settings
{
    internal class PluginTypeToIconConverter : IValueConverter
    {
        private PluginTypeToIconConverter()
        {
        }

        public static PluginTypeToIconConverter Instance { get; } = new PluginTypeToIconConverter();

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
                    LogTo.DebugException($"Failed to parse geometry data from plugin: {plugin.GetType()}", e);
                }
            }

            return Geometry.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}