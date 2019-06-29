using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal class ElementFadeIn : TweenTriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(TargetElement);

            TargetElement.Opacity = 0;
            TargetElement.Visibility = Visibility.Visible;

            var tweens = new List<Timeline>();

            if (DoScale)
            {
                var scaleTransform = new ScaleTransform(0.5, 0.5, Anchor.Position(TargetElement).X, Anchor.Position(TargetElement).Y);
                tweens.Add(scaleTransform.Tween(x => x.ScaleX).And(x => x.ScaleY).To(1.0).In(Duration).Ease(Easing.BackEaseOut));
                TargetElement.RenderTransform = scaleTransform;
            }

            tweens.Add(TargetElement.Tween(x => x.Opacity).To(1).In(Duration));
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(TargetElement));
        }
    }
}