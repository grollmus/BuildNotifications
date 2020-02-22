using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;

namespace BuildNotifications.ViewModel.Settings
{
    internal class SetupViewModel : BaseViewModel
    {
        public SetupViewModel(IConfiguration configuration, IPluginRepository pluginRepository)
        {
            Sections = new SetupSectionViewModel[]
            {
                new ConnectionsSectionViewModel(configuration, pluginRepository),
                new ProjectsSectionViewModel(configuration, pluginRepository)
            };
        }

        public IEnumerable<SetupSectionViewModel> Sections { get; }

        public SetupSectionViewModel SelectedItem => Sections.First();
    }
}