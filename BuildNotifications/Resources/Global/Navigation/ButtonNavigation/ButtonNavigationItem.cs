using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel;

namespace BuildNotifications.Resources.Global.Navigation.ButtonNavigation
{
    internal abstract class ButtonNavigationItem : BaseViewModel, IButtonNavigationItem
    {
        public abstract string DisplayNameTextId { get; }
        public abstract IconType IconType { get; }
    }
}