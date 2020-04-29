using System;
using System.Collections.Generic;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings.Setup
{
    internal class ConnectionViewModel : ButtonNavigationItem
    {
        public ConnectionViewModel(ConnectionData model, IPluginRepository pluginRepository)
        {
            Model = model;
            PluginRepository = pluginRepository;
            TestConnection = new TestConnectionViewModel(PluginRepository);
            SaveConnectionCommand = new DelegateCommand(SaveConnection);

            ConnectionPluginType = model.ConnectionType;
            SelectedPlugin = SelectPluginFromModel();
        }

        internal ConnectionViewModel(ConnectionData model, IPluginRepository pluginRepository, TestConnectionViewModel testConnection)
            : this(model, pluginRepository)
        {
            TestConnection = testConnection;
        }

        public IEnumerable<ConnectionPluginType> AvailableConnectionTypes
        {
            get
            {
                yield return ConnectionPluginType.Build;
                yield return ConnectionPluginType.SourceControl;
            }
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
        public override IconType Icon => IconType.None;
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
                RestoreConfiguration();
            }
        }

        public TestConnectionViewModel TestConnection { get; }
        public virtual event EventHandler<EventArgs>? SaveRequested;

        public void OnSelected()
        {
            RestoreConfiguration();
        }

        private void OnConfigurationOptionChanged(object? sender, EventArgs e)
        {
            SaveConnection();
        }

        private void RaiseSaveRequested()
        {
            SaveRequested?.Invoke(this, EventArgs.Empty);
        }

        private void RestoreConfiguration()
        {
            if (_selectedPlugin == null)
                SelectedPlugin = SelectPluginFromModel();

            if (_selectedPlugin != null)
            {
                if (PluginConfiguration != null)
                    PluginConfiguration.ValueChanged -= OnConfigurationOptionChanged;

                var config = _selectedPlugin.Configuration;
                config.Deserialize(Model.PluginConfiguration ?? string.Empty);
                PluginConfiguration = new PluginConfigurationViewModel(config);
                PluginConfiguration.ValueChanged += OnConfigurationOptionChanged;
                TestConnection.SetConfiguration(_selectedPlugin, config, ConnectionPluginType);
            }
        }

        private void SaveConnection()
        {
            if (_selectedPlugin != null && PluginConfiguration != null)
            {
                var serialized = PluginConfiguration.Configuration.Serialize();

                Model.ConnectionType = ConnectionPluginType;
                Model.PluginType = _selectedPlugin.GetType().FullName;
                Model.PluginConfiguration = serialized;

                RaiseSaveRequested();
            }
        }

        private IPlugin? SelectPluginFromModel()
        {
            return ConnectionPluginType switch
            {
                ConnectionPluginType.SourceControl => PluginRepository.FindSourceControlPlugin(Model.PluginType),
                ConnectionPluginType.Build => PluginRepository.FindBuildPlugin(Model.PluginType),
                _ => null
            };
        }

        private IPlugin? _selectedPlugin;
        private PluginConfigurationViewModel? _pluginConfiguration;
        private ConnectionPluginType _connectionPluginType;
    }
}