using System.Globalization;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration
{
    /// <summary>
    /// Used to localize strings from the plugin for different locales.
    /// </summary>
    [PublicAPI]
    public interface ILocalizer
    {
        /// <summary>
        /// Localizes a given Text Id for a given culture.
        /// </summary>
        /// <param name="culture">Culture to localize for.</param>
        /// <param name="id">Id of the text to localize.</param>
        /// <returns>The localized string or <c>null</c> if no localization exists for given culture.</returns>
        string Localized(string id, CultureInfo culture);
    }
}