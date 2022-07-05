using System.Collections.Generic;
using System.Windows;
using System.Windows.Shapes;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation.Checkbox
{
    internal class CheckboxBobbleMovementAnimation : TweenTriggerAction<Ellipse>
    {
        public bool IsChecked { get; set; }

        protected override void Invoke(object parameter)
        {
            var currentMargin = TargetElement.Margin;
            var targetMargin = IsChecked ? CheckboxSelectedToMarginConverter.CheckedMargin : CheckboxSelectedToMarginConverter.UncheckedMargin;

            var globalTweenHandler = App.GlobalTweenHandler;

            globalTweenHandler.ClearTweensOf(TargetElement);

            var tweens = new List<Timeline>();

            var wrapper = new DoubleWrapper {Value = currentMargin.Left};

            tweens.Add(wrapper.Tween(x => x.Value).To(targetMargin.Left).In(Duration).Ease(Easing.ExpoEaseOut).OnUpdate((_, _) => { TargetElement.Margin = new Thickness(wrapper.Value, 0, 0, 0); }));

            globalTweenHandler.Add(tweens.ToSequenceWithTarget(TargetElement));
        }

        private sealed class DoubleWrapper
        {
            public double Value { get; set; }
        }
    }
}