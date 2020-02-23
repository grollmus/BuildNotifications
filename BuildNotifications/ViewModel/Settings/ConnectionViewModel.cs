using System;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings
{
    internal class ConnectionViewModel : ButtonNavigationItem
    {
        public ConnectionViewModel(ConnectionData model, IPluginRepository pluginRepository)
        {
            Model = model;
            PluginRepository = pluginRepository;
            TestConnection = new TestConnectionViewModel(PluginRepository);
            SaveConnectionCommand = new DelegateCommand(SaveConnection);
        }

        public ConnectionPluginType ConnectionPluginType
        {
            get => _connectionPluginType;
            set
            {
                if (_connectionPluginType == value)
                    return;

                _connectionPluginType = value;
                OnPropertyChanged();
            }
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
                OnPropertyChanged(nameof(DisplayNameTextId));
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

        public ICommand SaveConnectionCommand { get; }

        public IPlugin? SelectedPlugin
        {
            get => _selectedPlugin;
            set
            {
                if (_selectedPlugin == value)
                    return;

                _selectedPlugin = value;
                OnPropertyChanged();

                if (_selectedPlugin != null)
                {
                    var config = _selectedPlugin.Configuration;
                    config.Deserialize(Model.PluginConfiguration ?? string.Empty);
                    PluginConfiguration = new PluginConfigurationViewModel(config);
                    TestConnection.SetConfiguration(_selectedPlugin, config, ConnectionPluginType);
                }
            }
        }

        public TestConnectionViewModel TestConnection { get; }
        public event EventHandler? SaveRequested;

        private void RaiseSaveRequested()
        {
            SaveRequested?.Invoke(this, EventArgs.Empty);
        }

        private void SaveConnection()
        {
            if (_selectedPlugin != null)
            {
                var config = _selectedPlugin.Configuration;
                var serialized = config.Serialize();

                Model.ConnectionType = ConnectionPluginType;
                Model.PluginType = _selectedPlugin.GetType().FullName;
                Model.PluginConfiguration = serialized;

                RaiseSaveRequested();
            }
        }

        private IPlugin? _selectedPlugin;
        private PluginConfigurationViewModel? _pluginConfiguration;
        private ConnectionPluginType _connectionPluginType;
    }
}