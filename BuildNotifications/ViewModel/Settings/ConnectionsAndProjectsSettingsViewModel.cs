using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using BuildNotifications.Core;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Text;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Resources.Icons;
using BuildNotifications.Resources.Settings;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings
{
    internal class ConnectionsAndProjectsSettingsViewModel : BaseViewModel
    {
        public ConnectionsAndProjectsSettingsViewModel(SettingsSubSetViewModel connectionSettings, SettingsSubSetViewModel projectsSettings, IPluginRepository pluginRepository)
        {
            _connectionSettings = connectionSettings;
            _pluginRepository = pluginRepository;
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

            TestConnectionCommand = AsyncCommand.Create(TestConnection);
        }

        public ObservableCollection<IButtonNavigationItem> Items { get; set; }
        public IAsyncCommand TestConnectionCommand { get; }

        public string TestConnectionTextId { get; } = "TestConnection";

        private async Task TestConnection()
        {
            await new SynchronizationContextRemover();

            var selectedConnection = _connectionSettings.Configs.FirstOrDefault();
            var data = selectedConnection?.Value as List<ConnectionData>;
            var selectedBuildData = data?.LastOrDefault();

            if (selectedBuildData == null)
                return;

            await TestConnection(selectedBuildData);
        }

        private async Task TestConnection(ConnectionData connectionData)
        {
            var buildPlugin = _pluginRepository.FindBuildPlugin(connectionData.BuildPluginType);
            var sourcePlugin = _pluginRepository.FindSourceControlPlugin(connectionData.SourceControlPluginType);
            var failed = false;

            if (buildPlugin != null && connectionData.BuildPluginConfiguration != null)
            {
                var buildResult = await buildPlugin.TestConnection(connectionData.BuildPluginConfiguration);

                if (!buildResult.IsSuccess)
                {
                    failed = true;

                    var caption = StringLocalizer.Instance["BuildConnectionTestFailed"];
                    MessageBox.Show(buildResult.ErrorMessage, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (sourcePlugin != null && connectionData.SourceControlPluginConfiguration != null)
            {
                var sourceResult = await sourcePlugin.TestConnection(connectionData.SourceControlPluginConfiguration);

                if (!sourceResult.IsSuccess)
                {
                    failed = true;

                    var caption = StringLocalizer.Instance["SourceConnectionTestFailed"];
                    MessageBox.Show(sourceResult.ErrorMessage, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            if (!failed)
            {
                var text = StringLocalizer.Instance["ConnectionTestSuccessful"];
                var caption = StringLocalizer.Instance["ConnectionTestCaption"];
                MessageBox.Show(text, caption, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private readonly SettingsSubSetViewModel _connectionSettings;
        private readonly IPluginRepository _pluginRepository;
    }
}