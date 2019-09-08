using System.Collections.ObjectModel;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.Settings
{
    internal class ConnectionsAndProjectsSettingsViewModel : BaseViewModel
    {
        public ConnectionsAndProjectsSettingsViewModel(SettingsSubSetViewModel connectionSettings, SettingsSubSetViewModel projectsSettings, IPluginRepository pluginRepository)
        {
            Items = new ObservableCollection<IButtonNavigationItem>();

            foreach (var config in connectionSettings.Configs)
            {
                config.AdditionalData = pluginRepository;
            }

            foreach (var config in projectsSettings.Configs)
            {
                config.AdditionalData = pluginRepository;
            }

            Items.Add(new ButtonNavigationItem(connectionSettings, "Connections", IconType.Connection));
            Items.Add(new ButtonNavigationItem(projectsSettings, "Projects", IconType.Project));
        }

        public ObservableCollection<IButtonNavigationItem> Items { get; set; }
    }
}