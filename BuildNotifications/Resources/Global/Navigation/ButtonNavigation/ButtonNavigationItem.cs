using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel;

namespace BuildNotifications.Resources.Global.Navigation.ButtonNavigation
{
    internal abstract class BaseButtonNavigationItem : BaseViewModel, IButtonNavigationItem
    {
        public abstract string DisplayNameTextId { get; }
        public abstract IconType Icon { get; }
    }
}
