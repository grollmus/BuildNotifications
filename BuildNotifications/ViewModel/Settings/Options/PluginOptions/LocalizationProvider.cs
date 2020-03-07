using System.Globalization;
using BuildNotifications.PluginInterfaces.Configuration;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
    internal class LocalizationProvider
        : ILocalizationProvider
    {
        public LocalizationProvider(ILocalizer localizer)
        {
            _localizer = localizer;

            _culture = CultureInfo.CurrentCulture;
        }

        public string Localize(string textId) => _localizer.Localized(textId, _culture);
        private readonly ILocalizer _localizer;
        private readonly CultureInfo _culture;
    }
}