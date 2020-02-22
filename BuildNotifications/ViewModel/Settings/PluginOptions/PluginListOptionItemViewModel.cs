using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginListOptionItemViewModel<TValue> : BaseViewModel
    {
        public PluginListOptionItemViewModel(ListOptionItem<TValue> item, ILocalizationProvider localizationProvider)
        {
            _item = item;
            _localizationProvider = localizationProvider;
        }

        public string Name => _localizationProvider.Localize(_item.DisplayName);
        public TValue Value => _item.Value;
        private readonly ListOptionItem<TValue> _item;
        private readonly ILocalizationProvider _localizationProvider;
    }
}