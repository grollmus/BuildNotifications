using System.Windows;
using System.Windows.Controls;

namespace BuildNotifications.Resources.Animation
{
    internal class AnimatedWrapPanel : WrapPanel
    {
        private readonly AnimatedPanelHelper Helper;

        public AnimatedWrapPanel()
        {
            Helper = new AnimatedPanelHelper();
        }

        public double AnimationDuration { get; set; } = 0.4;

        protected override Size ArrangeOverride(Size finalSize)
        {
            var positions = Helper.StoreOldPositions(this);
            var arrangedSize = base.ArrangeOverride(finalSize);
            Helper.AnimateToNewPositions(positions, this, AnimationDuration);
            return arrangedSize;
        }
    }
}