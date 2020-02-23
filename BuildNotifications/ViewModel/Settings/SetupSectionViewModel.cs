using System;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;

namespace BuildNotifications.ViewModel.Settings
{
    internal abstract class SetupSectionViewModel : ButtonNavigationItem
    {
        protected SetupSectionViewModel(IConfiguration configuration, IPluginRepository pluginRepository, Action saveAction)
        {
            Configuration = configuration;
            PluginRepository = pluginRepository;
            SaveAction = saveAction;
        }

        protected IConfiguration Configuration { get; }
        protected IPluginRepository PluginRepository { get; }
        protected Action SaveAction { get; }
    }
}