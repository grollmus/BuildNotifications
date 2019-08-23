using BuildNotifications.Resources.Icons;

namespace BuildNotifications.Resources.Global.Navigation.ButtonNavigation
{
    internal class ButtonNavigationItem : IButtonNavigationItem
    {
        public ButtonNavigationItem(object content, string displayNameTextId, IconType iconType = IconType.None)
        {
            Content = content;
            DisplayNameTextId = displayNameTextId;
            IconType = iconType;
        }

        public string DisplayNameTextId { get; }

        public IconType IconType { get; }

        public object Content { get; }
    }
}