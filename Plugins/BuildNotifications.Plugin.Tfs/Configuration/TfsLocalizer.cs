using System.Collections;
using System.Globalization;
using System.Linq;
using BuildNotifications.Plugin.Tfs.Resources;
using BuildNotifications.PluginInterfaces.Configuration;

namespace BuildNotifications.Plugin.Tfs.Configuration
{
    internal class TfsLocalizer : ILocalizer
    {
        public string Localized(string id, CultureInfo culture)
        {
            var resourceSet = Strings.ResourceManager.GetResourceSet(culture, true, true);
            var resource = resourceSet.Cast<DictionaryEntry>().FirstOrDefault(e => e.Key.ToString() == id);
            if (resource.Value == null)
                return $"[{id}]";

            return resource.Value?.ToString() ?? $"[{id}]";
        }
    }
}