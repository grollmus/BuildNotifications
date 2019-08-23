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
            var pluginRepo = values.LastOrDefault() as IPluginRepository;

            if (pluginType == null || pluginRepo == null)
                return null;

            return pluginRepo.FindPluginName(pluginType);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}