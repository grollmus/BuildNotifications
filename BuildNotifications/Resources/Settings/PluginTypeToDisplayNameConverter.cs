using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using BuildNotifications.Core.Plugin;

namespace BuildNotifications.Resources.Settings
{
    internal class PluginTypeToDisplayNameConverter : IMultiValueConverter
    {
        private PluginTypeToDisplayNameConverter()
        {
        }

        public static PluginTypeToDisplayNameConverter Instance { get; } = new PluginTypeToDisplayNameConverter();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var pluginType = values.FirstOrDefault()?.ToString();

            if (pluginType == null || !(values.LastOrDefault() is IPluginRepository pluginRepo))
                return "";

            return pluginRepo.FindPluginName(pluginType) ?? "";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}