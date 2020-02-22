using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Text;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.Settings
{
    internal class ConnectionsSectionViewModel : SetupSectionViewModel
    {
        public ConnectionsSectionViewModel(IConfiguration configuration, IPluginRepository pluginRepository)
            : base(configuration, pluginRepository)
        {
        }

        public override string DisplayNameTextId => StringLocalizer.Keys.Connections;
        public override IconType IconType => IconType.Connection;
    }
}