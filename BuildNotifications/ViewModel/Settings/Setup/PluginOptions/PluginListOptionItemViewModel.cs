using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Setup.PluginOptions
{
    internal class PluginListOptionItemViewModel<TValue> : BaseViewModel
    {
        public PluginListOptionItemViewModel(ListOptionItem<TValue> item, ILocalizationProvider localizationProvider)
        {
            _item = item;
            _localizationProvider = localizationProvider;

            _localizeName = typeof(TValue).IsEnum;
        }

        public string Name => _localizeName ? _localizationProvider.Localize(_item.DisplayName) : _item.DisplayName;
        public TValue Value => _item.Value;

        private readonly bool _localizeName;
        private readonly ListOptionItem<TValue> _item;
        private readonly ILocalizationProvider _localizationProvider;
    }
}