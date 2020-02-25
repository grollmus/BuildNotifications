using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.Setup.PluginOptions
{
    internal class PluginListOptionItemViewModel<TValue> : BaseViewModel
    {
        public PluginListOptionItemViewModel(ListOptionItem<TValue> item)
        {
            _item = item;
        }

        public string Name => _item.DisplayName;
        public TValue Value => _item.Value;
        private readonly ListOptionItem<TValue> _item;
    }
}