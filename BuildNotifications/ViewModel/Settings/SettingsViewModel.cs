using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using ReflectSettings;
using ReflectSettings.EditableConfigs;
using DelegateCommand = BuildNotifications.ViewModel.Utils.DelegateCommand;

namespace BuildNotifications.ViewModel.Settings
{
    public class SettingsViewModel
    {
        public SettingsViewModel(IConfiguration configuration, Action saveMethod)
        {
            Configuration = configuration;
            _saveMethod = saveMethod;
            EditConnectionsCommand = new DelegateCommand(OnEditConnections);

            CreateEditables();
        }

        public ObservableCollection<IEditableConfig> Configs { get; } = new ObservableCollection<IEditableConfig>();
        public IConfiguration Configuration { get; }

        public SettingsSubSetViewModel ConnectionsSubSet { get; private set; }

        public ICommand EditConnectionsCommand { get; set; }

        public SettingsSubSetViewModel ProjectsSubSet { get; private set; }

        public event EventHandler EditConnectionsRequested;

        public event EventHandler SettingsChanged;

        private void CreateEditables()
        {
            var factory = new SettingsFactory();
            var editables = factory.Reflect(Configuration, out var changeTrackingManager).ToList();

            var connectionEditables = new List<IEditableConfig>();
            var projectsEditables = new List<IEditableConfig>();

            foreach (var config in editables)
            {
                if (config.PropertyInfo.Name == nameof(IConfiguration.Connections))
                {
                    connectionEditables.Add(config);
                    continue;
                }

                if (config.PropertyInfo.Name == nameof(IConfiguration.Projects))
                {
                    projectsEditables.Add(config);
                    continue;
                }

                Configs.Add(config);
            }

            ConnectionsSubSet = new SettingsSubSetViewModel(connectionEditables);
            ProjectsSubSet = new SettingsSubSetViewModel(projectsEditables);

            changeTrackingManager.ConfigurationChanged += (sender, args) =>
            {
                _saveMethod.Invoke();
                SettingsChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        private void OnEditConnections(object parameter)
        {
            EditConnectionsRequested?.Invoke(this, EventArgs.Empty);
        }

        private readonly Action _saveMethod;
    }
}