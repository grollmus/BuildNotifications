using System;
using System.Collections.Generic;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.Settings.Setup
{
    internal class ConnectionViewModel : BaseButtonNavigationItem
    {
        public ConnectionViewModel(ConnectionData model, IPluginRepository pluginRepository)
        {
            Model = model;
            PluginRepository = pluginRepository;
            TestConnection = new TestConnectionViewModel(PluginRepository);

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
                SaveConnection();
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

        public IPlugin? SelectedPlugin
        {
            get => _selectedPlugin;
            set
            {
                _selectedPlugin = value;
                OnPropertyChanged();
                RestoreConfiguration(false);
            }
        }

        public TestConnectionViewModel TestConnection { get; }
        public virtual event EventHandler<EventArgs>? SaveRequested;

        public void OnDeselected()
        {
            if (PluginConfiguration != null)
            {
                PluginConfiguration.ValueChanged -= OnConfigurationOptionChanged;
                PluginConfiguration.Clear();
                _pluginConfiguration = null;
                _selectedPlugin = null;
            }
        }

        public void OnSelected()
        {
            _selectedPlugin = null;
            RestoreConfiguration(true);
        }

        private void OnConfigurationOptionChanged(object? sender, EventArgs e)
        {
            if (_inRestore)
                return;

            SaveConnection();
        }

        private void RaiseSaveRequested()
        {
            SaveRequested?.Invoke(this, EventArgs.Empty);
        }

        private void RestoreConfiguration(bool loadFromModel)
        {
            _inRestore = true;

            if (PluginConfiguration != null)
            {
                PluginConfiguration.ValueChanged -= OnConfigurationOptionChanged;
                PluginConfiguration.Clear();
            }

            if (loadFromModel)
                _selectedPlugin ??= SelectPluginFromModel();

            if (_selectedPlugin != null)
            {
                var config = _selectedPlugin.ConstructNewConfiguration();
                config.Deserialize(Model.PluginConfiguration ?? string.Empty);
                PluginConfiguration = new PluginConfigurationViewModel(config);
                PluginConfiguration.ValueChanged += OnConfigurationOptionChanged;
                TestConnection.SetConfiguration(_selectedPlugin, config, ConnectionPluginType);
            }

            _inRestore = false;
        }

        private void SaveConnection()
        {
            if (_inRestore)
                return;

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
        private bool _inRestore;
    }
}