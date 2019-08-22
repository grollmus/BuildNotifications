using System;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Settings;

namespace BuildNotifications.ViewModel.Overlays
{
    internal class InitialSetupOverlayViewModel : BaseViewModel
    {
        public InitialSetupOverlayViewModel(SettingsViewModel settingsViewModel, IPluginRepository pluginRepository)
        {
            _settingsViewModel = settingsViewModel;
            ConnectionsAndProjectsSettingsViewModel = new ConnectionsAndProjectsSettingsViewModel(settingsViewModel.ConnectionsSubSet, settingsViewModel.ProjectsSubSet, pluginRepository);
            ConnectionsAndProjectsSettingsViewModel.Items.Add(new ButtonNavigationItem("Placeholder. Restart BuildNotifications with setup Project and Connection to get to the default view", "Test", IconType.Dummy));
            settingsViewModel.SettingsChanged += UpdateText;
            UpdateText();
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

        public ConnectionsAndProjectsSettingsViewModel ConnectionsAndProjectsSettingsViewModel { get; set; }

        public IconType DisplayedIconType
        {
            get => _displayedIconType;
            set
            {
                _displayedIconType = value;
                OnPropertyChanged();
            }
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

        private readonly SettingsViewModel _settingsViewModel;
        private string _displayedTextId;
        private IconType _displayedIconType;
        private bool _animateDisplay;
        private const string InitialSetupCompleteConfig = nameof(InitialSetupCompleteConfig);

        private const string InitialSetupEmptyConf = nameof(InitialSetupEmptyConf);
        private const string InitialSetupEmptyConnections = nameof(InitialSetupEmptyConnections);
        private const string InitialSetupEmptyProjects = nameof(InitialSetupEmptyProjects);
    }
}