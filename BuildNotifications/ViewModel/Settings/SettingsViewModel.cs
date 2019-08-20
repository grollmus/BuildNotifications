using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BuildNotifications.Core.Config;
using ReflectSettings;
using ReflectSettings.EditableConfigs;

namespace BuildNotifications.ViewModel.Settings
{
    public class SettingsViewModel
    {
        private readonly IConfiguration _configuration;
        private readonly Action _saveMethod;

        public ObservableCollection<IEditableConfig> Configs { get; } = new ObservableCollection<IEditableConfig>();

        public SettingsViewModel(IConfiguration configuration, Action saveMethod)
        {
            _configuration = configuration;
            _saveMethod = saveMethod;

            CreateEditables();
        }

        private void CreateEditables()
        {
            var factory = new SettingsFactory();
            var editables = factory.Reflect(_configuration).ToList();

            foreach (var config in editables)
            {
                Configs.Add(config);
                config.PropertyChanged += ConfigOnPropertyChanged;
            }
        }

        private void ConfigOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _saveMethod?.Invoke();
        }
    }
}