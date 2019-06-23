using System.Windows;
using System.Windows.Interactivity;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal abstract class TweenTriggerAction : TriggerAction<UIElement>
    {
        public double Duration { get; set; } = 1;

        public Anchor Anchor { get; set; } = Anchor.Center;

        public bool DoScale { get; set; } = true;
    }
}
