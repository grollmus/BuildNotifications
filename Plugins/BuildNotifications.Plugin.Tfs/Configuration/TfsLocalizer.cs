using System.Globalization;
using BuildNotifications.Plugin.Tfs.Resources;
using BuildNotifications.PluginInterfaces.Configuration;

namespace BuildNotifications.Plugin.Tfs.Configuration
{
    internal class TfsLocalizer : ILocalizer
    {
        public string Localized(string id, CultureInfo culture)
        {
            return Strings.ResourceManager.GetString(id, culture) ?? id;
        }
    }
}