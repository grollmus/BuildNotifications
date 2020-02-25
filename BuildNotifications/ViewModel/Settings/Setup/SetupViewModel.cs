using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;

namespace BuildNotifications.ViewModel.Settings.Setup
{
    internal class SetupViewModel : BaseViewModel
    {
        public SetupViewModel(IConfiguration configuration, IPluginRepository pluginRepository, Action saveAction, IConfigurationBuilder configurationBuilder)
        {
            Sections = new SetupSectionViewModel[]
            {
                new ConnectionsSectionViewModel(configuration, pluginRepository, saveAction),
                new ProjectsSectionViewModel(configurationBuilder, configuration, saveAction)
            };
        }

        public IEnumerable<SetupSectionViewModel> Sections { get; }

        public SetupSectionViewModel SelectedItem => Sections.First();
    }
}