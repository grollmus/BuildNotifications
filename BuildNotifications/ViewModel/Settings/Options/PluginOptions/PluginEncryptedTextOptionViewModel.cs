using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
    internal class PluginEncryptedTextOptionViewModel : OptionViewModelBase<PasswordString?>, IPluginOptionViewModel
    {
        public PluginEncryptedTextOptionViewModel(ValueOption<PasswordString?> valueOption, ILocalizationProvider localizationProvider)
            : base(valueOption.Value, valueOption.NameTextId)
        {
            _pluginOptionViewModelImplementation = new PluginOptionViewModelImplementation<PasswordString?>(valueOption, localizationProvider, this);
        }

        public string RawValue
        {
            get => Value?.PlainText() ?? string.Empty;
            set => Value = PasswordString.FromPlainText(value);
        }

        public override PasswordString? Value
        {
            get => _pluginOptionViewModelImplementation.Value;
            set => _pluginOptionViewModelImplementation.Value = value;
        }

        private readonly PluginOptionViewModelImplementation<PasswordString?> _pluginOptionViewModelImplementation;
    }
}