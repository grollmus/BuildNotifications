using System;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Settings;

namespace BuildNotifications.ViewModel.Overlays
{
    internal class InitialSetupOverlayViewModel : BaseViewModel
    {
        private readonly SettingsViewModel _settingsViewModel;
        private string _displayedTextId;
        private IconType _displayedIconType;
        private bool _animateDisplay;
        public ConnectionsAndProjectsSettingsViewModel ConnectionsAndProjectsSettingsViewModel { get; set; }

        public InitialSetupOverlayViewModel(SettingsViewModel settingsViewModel, IPluginRepository pluginRepository)
        {
            _settingsViewModel = settingsViewModel;
            ConnectionsAndProjectsSettingsViewModel = new ConnectionsAndProjectsSettingsViewModel(settingsViewModel.ConnectionsSubSet, settingsViewModel.ProjectsSubSet, pluginRepository);
            ConnectionsAndProjectsSettingsViewModel.Items.Add(new ButtonNavigationItem("Placeholder. Restart BuildNotifications with setup Project and Connection to get to the default view", "Test", IconType.Dummy));
            settingsViewModel.SettingsChanged += UpdateText;
            UpdateText();
        }

        public string DisplayedTextId
        {
            get => _displayedTextId;
            set
            {
                if (_displayedTextId == value)
                    return;

                _displayedTextId = value;
                AnimateDisplay = true;
                AnimateDisplay = false;
                OnPropertyChanged();
            }
        }

        public IconType DisplayedIconType
        {
            get => _displayedIconType;
            set
            {
                _displayedIconType = value;
                OnPropertyChanged();
            }
        }

        public bool AnimateDisplay
        {
            get => _animateDisplay;
            set
            {
                _animateDisplay = value;
                OnPropertyChanged();
            }
        }

        private const string InitialSetupEmptyConf = nameof(InitialSetupEmptyConf);
        private const string InitialSetupEmptyProjects = nameof(InitialSetupEmptyProjects);
        private const string InitialSetupEmptyConnections = nameof(InitialSetupEmptyConnections);
        private const string InitialSetupCompleteConfig = nameof(InitialSetupCompleteConfig);

        private void UpdateText(object sender = null, EventArgs e = null)
        {
            if (_settingsViewModel.Configuration.Connections.Count == 0 && _settingsViewModel.Configuration.Projects.Count == 0)
            {
                DisplayedTextId = InitialSetupEmptyConf;
                DisplayedIconType = IconType.Status;
                return;
            }

            if (_settingsViewModel.Configuration.Connections.Count == 0)
            {
                DisplayedTextId = InitialSetupEmptyConnections;
                DisplayedIconType = IconType.Connection;
                return;
            }

            if (_settingsViewModel.Configuration.Projects.Count == 0)
            {
                DisplayedTextId = InitialSetupEmptyProjects;
                DisplayedIconType = IconType.GroupingEmpty;
                return;
            }

            DisplayedTextId = InitialSetupCompleteConfig;
            DisplayedIconType = IconType.Settings;
        }
    }
}