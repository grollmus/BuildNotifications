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

        public override string Description => _pluginOptionViewModelImplementation.Description;
        public override string DisplayName => _pluginOptionViewModelImplementation.DisplayName;
        public override bool IsEnabled => _pluginOptionViewModelImplementation.IsEnabled;
        public override bool IsLoading => _pluginOptionViewModelImplementation.IsLoading;
        public override bool IsVisible => _pluginOptionViewModelImplementation.IsVisible;

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