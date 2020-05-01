using System;
using BuildNotifications.Core.Config;
using BuildNotifications.Resources.Global.Navigation.ButtonNavigation;
using BuildNotifications.Services;

namespace BuildNotifications.ViewModel.Settings.Setup
{
    internal abstract class SetupSectionViewModel : BaseButtonNavigationItem
    {
        protected SetupSectionViewModel(IConfiguration configuration, Action saveAction, IPopupService popupService)
        {
            Configuration = configuration;
            SaveAction = saveAction;
            PopupService = popupService;
        }

        protected IConfiguration Configuration { get; }
        protected IPopupService PopupService { get; }
        protected Action SaveAction { get; }

        public event EventHandler? Changed;

        protected void RaiseChanged()
        {
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}