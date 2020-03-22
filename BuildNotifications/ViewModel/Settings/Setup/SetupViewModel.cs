using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Services;

namespace BuildNotifications.ViewModel.Settings.Setup
{
    internal class SetupViewModel : BaseViewModel
    {
        public SetupViewModel(IConfiguration configuration, IPluginRepository pluginRepository, Action saveAction, IConfigurationBuilder configurationBuilder, IPopupService popupService)
        {
            Connections = new ConnectionsSectionViewModel(configuration, pluginRepository, saveAction, popupService);
            Projects = new ProjectsSectionViewModel(configurationBuilder, configuration, saveAction, popupService);

            Sections = new SetupSectionViewModel[]
            {
                Connections,
                Projects
            };

            _selectedItem = Sections.First();
        }

        public ConnectionsSectionViewModel Connections { get; }
        public ProjectsSectionViewModel Projects { get; }

        public IEnumerable<SetupSectionViewModel> Sections { get; }

        public SetupSectionViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem == value)
                    return;

                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        private SetupSectionViewModel _selectedItem;
    }
}