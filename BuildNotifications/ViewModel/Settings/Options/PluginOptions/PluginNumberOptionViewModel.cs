using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Options.PluginOptions
{
    internal class PluginNumberOptionViewModel : NumberOptionViewModel, IPluginOptionViewModel
    {
        public PluginNumberOptionViewModel(NumberOption option, ILocalizationProvider localizationProvider)
            : base(option.Value, option.MinValue, option.MaxValue, option.NameTextId)
        {
            _pluginOptionViewModelImplementation = new PluginOptionViewModelImplementation<int>(option, localizationProvider, this);
        }

        public override string Description => _pluginOptionViewModelImplementation.Description;
        public override string DisplayName => _pluginOptionViewModelImplementation.DisplayName;
        public override bool IsEnabled => _pluginOptionViewModelImplementation.IsEnabled;
        public override bool IsLoading => _pluginOptionViewModelImplementation.IsLoading;
        public override bool IsVisible => _pluginOptionViewModelImplementation.IsVisible;

        public override int Value
        {
            get => _pluginOptionViewModelImplementation.Value;
            set => _pluginOptionViewModelImplementation.Value = value;
        }

        public void Clear()
        {
            _pluginOptionViewModelImplementation.Clear();
        }

        private readonly PluginOptionViewModelImplementation<int> _pluginOptionViewModelImplementation;
    }
}