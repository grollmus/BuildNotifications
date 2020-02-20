using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    internal class PluginBooleanOptionViewModel : PluginValueOptionViewModel<bool>
    {
        public PluginBooleanOptionViewModel(BooleanOption valueOption)
            : base(valueOption)
        {
        }
    }
}