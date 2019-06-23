using System.Windows;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal static class AnchorExtension
    {
        public static Size Position(this Anchor anchor, UIElement forElement)
        {
            var size = forElement.RenderSize;

            switch (anchor)
            {
                default:
                case Anchor.Center:
                    return new Size(size.Width / 2, size.Height / 2);
                case Anchor.MiddleLeft:
                    return new Size(0, size.Height / 2);
                case Anchor.MiddleRight:
                    return new Size(size.Width, size.Height / 2);
            }
        }
    }
}