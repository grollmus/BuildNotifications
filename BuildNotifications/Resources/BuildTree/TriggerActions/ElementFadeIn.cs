using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal class ElementFadeIn : TweenTriggerAction<FrameworkElement>
    {
        public double Delay { get; set; } = 0.4;

        public bool DoCollapsingLayoutTransform { get; set; }

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(TargetElement);

            TargetElement.Opacity = 0;
            TargetElement.Visibility = Visibility.Visible;

            if (DoScale)
            {
                var tweens = new List<Timeline>();
                var scaleTransform = new ScaleTransform(0.5, 1, Anchor.Position(TargetElement).X, Anchor.Position(TargetElement).Y);
                tweens.Add(scaleTransform.Tween(x => x.ScaleX).And(x => x.ScaleY).To(1.0).In(Duration).Ease(Easing.ExpoEaseOut));
                TargetElement.RenderTransform = scaleTransform;
                tweens.Add(TargetElement.Tween(x => x.Opacity).To(1).In(Duration).Delay(Delay));
                globalTweenHandler.Add(tweens.ToSequenceWithTarget(TargetElement));
            }
            else
                globalTweenHandler.Add(TargetElement.Tween(x => x.Opacity).To(1).In(Duration).Delay(Delay));
            
            if (DoCollapsingLayoutTransform)
            {
                var scaleTransform = new ScaleTransform(0, 1.0, Anchor.Position(TargetElement).X, Anchor.Position(TargetElement).Y);
                globalTweenHandler.Add(scaleTransform.Tween(x => x.ScaleX).To(1).In(Duration).Ease(Easing.ExpoEaseIn));
                TargetElement.LayoutTransform = scaleTransform;
            }
        }
    }
}