using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginTextOptionViewModel : PluginValueOptionViewModel<string?>
    {
        public PluginTextOptionViewModel(TextOption valueOption, ILocalizationProvider localizationProvider)
            : base(valueOption, localizationProvider)
        {
        }
    }
}