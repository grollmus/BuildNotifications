using BuildNotifications.PluginInterfaces.Configuration.Options;

namespace BuildNotifications.ViewModel.Settings.PluginOptions
{
    public abstract class PluginOptionViewModel : BaseViewModel
    {
        protected PluginOptionViewModel(IOption option)
        {
            Option = option;
        }

        public string Description => Option.DescriptionTextId;
        public string DisplayName => Option.NameTextId;
        public bool IsEnabled => Option.IsEnabled;
        public bool IsVisible => Option.IsVisible;

        protected IOption Option { get; }
    }
}