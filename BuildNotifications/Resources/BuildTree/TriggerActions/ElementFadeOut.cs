using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal class ElementFadeOut : TweenTriggerAction<FrameworkElement>
    {
        public bool DoCollapsingLayoutTransform { get; set; }

        public ElementFadeOut()
        {
            Anchor = Anchor.MiddleLeft;
            Duration = 0.15;
        }

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(TargetElement);

            var tweens = new List<Timeline>();

            if (DoScale)
            {
                var scaleTransform = new ScaleTransform(1.0, 1.0, Anchor.Position(TargetElement).X, Anchor.Position(TargetElement).Y);
                tweens.Add(scaleTransform.Tween(x => x.ScaleX).And(x => x.ScaleY).To(0.5).In(Duration).Ease(Easing.ExpoEaseIn));
                TargetElement.RenderTransform = scaleTransform;
            }

            if (DoCollapsingLayoutTransform)
            {
                var scaleTransform = new ScaleTransform(1.0, 1.0, Anchor.Position(TargetElement).X, Anchor.Position(TargetElement).Y);
                tweens.Add(scaleTransform.Tween(x => x.ScaleX).To(0).In(Duration).Ease(Easing.ExpoEaseIn));
                TargetElement.LayoutTransform = scaleTransform;
            }

            tweens.Add(TargetElement.Tween(x => x.Opacity).To(0).In(Duration));
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(TargetElement).OnComplete((_, _) => { TargetElement.Visibility = Visibility.Hidden; }));
        }
    }
}