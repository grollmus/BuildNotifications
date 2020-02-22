using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginEncryptedTextOptionViewModel : PluginValueOptionViewModel<PasswordString?>
    {
        public PluginEncryptedTextOptionViewModel(ValueOption<PasswordString?> valueOption, ILocalizationProvider localizationProvider)
            : base(valueOption, localizationProvider)
        {
        }
    }
}