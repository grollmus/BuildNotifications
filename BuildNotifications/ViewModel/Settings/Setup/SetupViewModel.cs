using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core;
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

            foreach (var section in Sections)
            {
                section.Changed += Section_Changed;
            }
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

        private void Section_Changed(object? sender, EventArgs e)
        {
            var senderSection = sender as SetupSectionViewModel;
            var otherSections = Sections.Except(senderSection.Yield()).WhereNotNull();

            foreach (var otherSection in otherSections)
            {
                otherSection!.Refresh();
            }
        }

        private SetupSectionViewModel _selectedItem;
    }
}