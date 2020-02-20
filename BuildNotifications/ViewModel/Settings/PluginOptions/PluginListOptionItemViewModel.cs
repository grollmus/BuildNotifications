using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginListOptionItemViewModel<TValue> : BaseViewModel
    {
        public PluginListOptionItemViewModel(ListOptionItem<TValue> item)
        {
            Name = item.DisplayName;
            Value = item.Value;
        }

        public string Name { get; }
        public TValue Value { get; }
    }
}