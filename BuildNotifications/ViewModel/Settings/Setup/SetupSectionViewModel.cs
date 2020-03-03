using System;
using BuildNotifications.Core.Config;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;

namespace BuildNotifications.ViewModel.Settings.Setup
{
    internal abstract class SetupSectionViewModel : ButtonNavigationItem
    {
        protected SetupSectionViewModel(IConfiguration configuration, Action saveAction)
        {
            Configuration = configuration;
            SaveAction = saveAction;
        }

        protected IConfiguration Configuration { get; }
        protected Action SaveAction { get; }

        public event EventHandler? Changed;

        protected void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}