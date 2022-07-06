using System.Windows;

namespace BuildNotifications.Resources.BuildTree.TriggerActions;

internal static class AnchorExtension
{
    public static Point Position(this Anchor anchor, UIElement forElement)
    {
        var size = forElement.RenderSize;

        switch (anchor)
        {
            default:
                return new Point(size.Width / 2, size.Height / 2);
            case Anchor.MiddleLeft:
                return new Point(0, size.Height / 2);
            case Anchor.MiddleRight:
                return new Point(size.Width, size.Height / 2);
        }
    }
}