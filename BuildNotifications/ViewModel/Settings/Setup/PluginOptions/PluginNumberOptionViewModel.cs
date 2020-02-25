using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Setup.PluginOptions
{
    internal class PluginNumberOptionViewModel : PluginValueOptionViewModel<int>
    {
        public PluginNumberOptionViewModel(NumberOption option, ILocalizationProvider localizationProvider)
            : base(option, localizationProvider)
        {
        }
    }
}