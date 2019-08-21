using System.Collections.ObjectModel;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Resources.Icons;
using BuildNotifications.Resources.Settings;

namespace BuildNotifications.ViewModel.Settings
{
    internal class ConnectionsAndProjectsSettingsViewModel : BaseViewModel
    {
        public ObservableCollection<IButtonNavigationItem> Items { get; set; }

        public ConnectionsAndProjectsSettingsViewModel(SettingsSubSetViewModel connectionSettings, SettingsSubSetViewModel projectsSettings, IPluginRepository pluginRepository)
        {
            Items = new ObservableCollection<IButtonNavigationItem>();
            connectionSettings.DataTemplateSelector = ConnectionsAndProjectsTemplateSelector.Instance;
            projectsSettings.DataTemplateSelector = ConnectionsAndProjectsTemplateSelector.Instance;

            foreach (var config in connectionSettings.Configs)
            {
                config.AdditionalData = pluginRepository;
            }

            foreach (var config in projectsSettings.Configs)
            {
                config.AdditionalData = pluginRepository;
            }

            Items.Add(new ButtonNavigationItem(connectionSettings, "Connections", IconType.Connection));
            Items.Add(new ButtonNavigationItem(projectsSettings, "Projects", IconType.GroupingEmpty));
        }
    }
}