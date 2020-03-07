using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.ViewModel.Settings.Options.PluginOptions;

namespace BuildNotifications.ViewModel.Settings.Setup
{
    internal class PluginConfigurationViewModel : BaseViewModel
    {
        public PluginConfigurationViewModel(IPluginConfiguration configuration)
        {
            Options = new ObservableCollection<IPluginOptionViewModel>(ConstructOptionViewModels(configuration));
        }

        public ObservableCollection<IPluginOptionViewModel> Options { get; }

        private IEnumerable<IPluginOptionViewModel> ConstructOptionViewModels(IPluginConfiguration configuration)
        {
            var factory = new PluginOptionViewModelFactory(configuration.Localizer);
            return configuration.ListAvailableOptions().Select(o => factory.Construct(o));
        }
    }
}