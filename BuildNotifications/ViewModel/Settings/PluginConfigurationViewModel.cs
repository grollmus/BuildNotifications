using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.Resources.Settings;
using BuildNotifications.ViewModel.Settings.PluginOptions;

namespace BuildNotifications.ViewModel.Settings
{
    internal class PluginConfigurationViewModel : BaseViewModel
    {
        public PluginConfigurationViewModel(IPluginConfiguration configuration, ConnectionData model, PluginType pluginType)
        {
            _configuration = configuration;
            _model = model;
            _pluginType = pluginType;
            Options = new ObservableCollection<PluginOptionViewModel>(ConstructOptionViewModels(configuration));

            foreach (var option in Options)
            {
                option.PropertyChanged += Option_PropertyChanged;
            }
        }

        public ObservableCollection<PluginOptionViewModel> Options { get; }

        private IEnumerable<PluginOptionViewModel> ConstructOptionViewModels(IPluginConfiguration configuration)
        {
            var factory = new PluginOptionViewModelFactory(configuration.Localizer);
            return configuration.ListAvailableOptions().Select(o => factory.Construct(o));
        }

        private void Option_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PluginValueOptionViewModel<object>.Value))
            {
                var serialized = _configuration.Serialize();

                if (_pluginType == PluginType.Build)
                    _model.BuildPluginConfiguration = serialized;
                else if (_pluginType == PluginType.SourceControl)
                    _model.SourceControlPluginConfiguration = serialized;
            }
        }

        private readonly IPluginConfiguration _configuration;
        private readonly ConnectionData _model;
        private readonly PluginType _pluginType;
    }
}