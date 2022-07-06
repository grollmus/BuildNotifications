using BuildNotifications.Resources.Icons;

namespace BuildNotifications.Resources.Global.Navigation.ButtonNavigation;

internal interface IButtonNavigationItem
{
    string DisplayNameTextId { get; }
    IconType Icon { get; }
}