using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.Settings
{
    internal class ConnectionViewModel : ButtonNavigationItem
    {
        public ConnectionViewModel(ConnectionData model, IPluginRepository pluginRepository)
        {
            Model = model;
            PluginRepository = pluginRepository;
        }

        public override string DisplayNameTextId => Name;
        public override IconType IconType => IconType.None;
        public ConnectionData Model { get; }

        public string Name
        {
            get => Model.Name;
            set
            {
                if (Model.Name == value)
                    return;

                Model.Name = value;
                OnPropertyChanged();
            }
        }

        public PluginConfigurationViewModel? PluginConfiguration
        {
            get => _pluginConfiguration;
            private set
            {
                if (_pluginConfiguration == value)
                    return;

                _pluginConfiguration = value;
                OnPropertyChanged();
            }
        }

        public IPluginRepository PluginRepository { get; }

        public IPlugin? SelectedBuildPlugin
        {
            get => _selectedBuildPlugin;
            set
            {
                if (_selectedBuildPlugin == value)
                    return;

                _selectedBuildPlugin = value;
                OnPropertyChanged();

                if (_selectedBuildPlugin != null)
                {
                    var config = _selectedBuildPlugin.Configuration;
                    config.Deserialize(Model.BuildPluginConfiguration ?? string.Empty);
                    PluginConfiguration = new PluginConfigurationViewModel(config);
                }
            }
        }

        private IPlugin? _selectedBuildPlugin;
        private PluginConfigurationViewModel? _pluginConfiguration;
    }
}