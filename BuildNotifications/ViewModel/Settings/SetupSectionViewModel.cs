using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;

namespace BuildNotifications.ViewModel.Settings
{
    internal abstract class SetupSectionViewModel : ButtonNavigationItem
    {
        protected SetupSectionViewModel(IConfiguration configuration, IPluginRepository pluginRepository)
        {
            Configuration = configuration;
            PluginRepository = pluginRepository;
        }

        protected IConfiguration Configuration { get; }
        protected IPluginRepository PluginRepository { get; }
    }
}