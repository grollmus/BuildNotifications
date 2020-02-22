using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginDisplayOptionViewModel : PluginOptionViewModel
    {
        public PluginDisplayOptionViewModel(IOption option, ILocalizationProvider localizationProvider)
            : base(option, localizationProvider)
        {
            Value = option.ToString() ?? string.Empty;
        }

        public string Value { get; }
    }
}