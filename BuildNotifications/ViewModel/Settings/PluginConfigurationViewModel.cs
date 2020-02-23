using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.ViewModel.Settings.PluginOptions;

namespace BuildNotifications.ViewModel.Settings
{
    internal class PluginConfigurationViewModel : BaseViewModel
    {
        public PluginConfigurationViewModel(IPluginConfiguration configuration)
        {
            Options = new ObservableCollection<PluginOptionViewModel>(ConstructOptionViewModels(configuration));
        }

        public ObservableCollection<PluginOptionViewModel> Options { get; }

        private IEnumerable<PluginOptionViewModel> ConstructOptionViewModels(IPluginConfiguration configuration)
        {
            var factory = new PluginOptionViewModelFactory(configuration.Localizer);
            return configuration.ListAvailableOptions().Select(o => factory.Construct(o));
        }
    }
}