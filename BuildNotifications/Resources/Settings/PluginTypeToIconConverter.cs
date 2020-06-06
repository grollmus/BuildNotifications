using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using BuildNotifications.Core.Plugin;
using NLog.Fluent;

namespace BuildNotifications.Resources.Settings
{
    internal class PluginTypeToIconConverter : IMultiValueConverter
    {
        private PluginTypeToIconConverter()
        {
        }

        public static PluginTypeToIconConverter Instance { get; } = new PluginTypeToIconConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var pluginType = values.FirstOrDefault()?.ToString();
            var pluginRepo = values.LastOrDefault() as IPluginRepository;

            if (pluginType == null || pluginRepo == null)
                return Geometry.Empty;

            var geometryString = pluginRepo.FindIconGeometry(pluginType);
            if (geometryString == null)
                return Geometry.Empty;

            try
            {
                return Geometry.Parse(geometryString);
            }
            catch (Exception e)
            {
                Log.Debug().Message($"Failed to parse geometry data from plugin: {pluginType}").Exception(e).Write();
                return Geometry.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}