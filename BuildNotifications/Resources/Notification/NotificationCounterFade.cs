using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Notification
{
    internal class NotificationCounterFade : TweenTriggerAction<UIElement>
    {
        public bool FadeIn { get; set; }

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(TargetElement);

            var scaleTransform = TargetElement.RenderTransform as ScaleTransform;

            var tweens = new List<Timeline>();
            if (FadeIn)
            {
                scaleTransform ??= new ScaleTransform(0, 0, 0, 0);
                tweens.Add(scaleTransform.Tween(x => x.ScaleX).To(1.0).In(1).Ease(Easing.BackEaseOut));
                tweens.Add(scaleTransform.Tween(x => x.ScaleY).To(1.0).In(0.8).Ease(Easing.BackEaseOut));
            }
            else
            {
                scaleTransform ??= new ScaleTransform(1.0, 1.0, 0, 0);
                tweens.Add(scaleTransform.Tween(x => x.ScaleX).To(0).In(0.4).Ease(Easing.BackEaseIn));
                tweens.Add(scaleTransform.Tween(x => x.ScaleY).To(0).In(0.8).Ease(Easing.BackEaseIn));
            }

            scaleTransform.CenterX = Anchor.Position(TargetElement).X;
            scaleTransform.CenterY = Anchor.Position(TargetElement).Y;

            globalTweenHandler.Add(tweens.ToSequenceWithTarget(TargetElement));

            TargetElement.RenderTransform = scaleTransform;
        }
    }
}