using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
    internal class PluginBooleanOptionViewModel : BooleanOptionViewModel, IPluginOptionViewModel
    {
        public PluginBooleanOptionViewModel(BooleanOption valueOption, ILocalizationProvider localizationProvider)
            : base(valueOption.Value, valueOption.NameTextId)
        {
            _pluginOptionViewModelImplementation = new PluginOptionViewModelImplementation<bool>(valueOption, localizationProvider, this);
        }

        public override string Description => _pluginOptionViewModelImplementation.Description;
        public override string DisplayName => _pluginOptionViewModelImplementation.DisplayName;
        public override bool IsEnabled => _pluginOptionViewModelImplementation.IsEnabled;
        public override bool IsLoading => _pluginOptionViewModelImplementation.IsLoading;
        public override bool IsVisible => _pluginOptionViewModelImplementation.IsVisible;

        public override bool Value
        {
            get => _pluginOptionViewModelImplementation.Value;
            set => _pluginOptionViewModelImplementation.Value = value;
        }

        private readonly PluginOptionViewModelImplementation<bool> _pluginOptionViewModelImplementation;
    }
}