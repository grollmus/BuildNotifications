using System;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.Core.Text;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.Settings
{
    internal class ProjectsSectionViewModel : SetupSectionViewModel
    {
        public ProjectsSectionViewModel(IConfiguration configuration, IPluginRepository pluginRepository, Action saveAction)
            : base(configuration, pluginRepository, saveAction)
        {
        }

        public override string DisplayNameTextId => StringLocalizer.Keys.Projects;
        public override IconType IconType => IconType.Project;
    }
}