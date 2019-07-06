using System.Windows;
using System.Windows.Controls;

namespace BuildNotifications.Resources.Animation
{
    internal class AnimatedStackPanel : StackPanel
    {
        private readonly AnimatedPanelHelper _helper;

        public AnimatedStackPanel()
        {
            _helper = new AnimatedPanelHelper();
        }

        public double AnimationDuration { get; set; } = 0.4;

        protected override Size ArrangeOverride(Size finalSize)
        {
            var positions = _helper.StoreOldPositions(this);
            var arrangedSize = base.ArrangeOverride(finalSize);
            _helper.AnimateToNewPositions(positions, this, AnimationDuration);
            return arrangedSize;
        }
    }
}