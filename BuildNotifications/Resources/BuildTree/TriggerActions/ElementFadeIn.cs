using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal class ElementFadeIn : TweenTriggerAction
    {
        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            AssociatedObject.Opacity = 0;
            AssociatedObject.Visibility = Visibility.Visible;

            var tweens = new List<Timeline>();

            if (DoScale)
            {
                var scaleTransform = new ScaleTransform(0.5, 0.5, Anchor.Position(AssociatedObject).X, Anchor.Position(AssociatedObject).Y);
                tweens.Add(scaleTransform.Tween(x => x.ScaleX).And(x => x.ScaleY).To(1.0).In(Duration).Ease(Easing.BackEaseOut));
                AssociatedObject.RenderTransform = scaleTransform;
            }

            tweens.Add(AssociatedObject.Tween(x => x.Opacity).To(1).In(Duration));
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(AssociatedObject));
        }
    }
}