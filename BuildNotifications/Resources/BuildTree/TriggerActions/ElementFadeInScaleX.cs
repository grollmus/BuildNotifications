using System.Collections.Generic;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal class ElementFadeInScaleX : TweenTriggerAction
    {
        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            AssociatedObject.Opacity = 0;

            var tweens = new List<Timeline>();

            if (DoScale)
            {
                var scaleTransform = new ScaleTransform(0, 1.0, Anchor.Position(AssociatedObject).Width, Anchor.Position(AssociatedObject).Height);
                tweens.Add(scaleTransform.Tween(x => x.ScaleX).To(1.0).In(Duration).Ease(Easing.QuadraticEaseInOut));
                AssociatedObject.RenderTransform = scaleTransform;
            }

            tweens.Add(AssociatedObject.Tween(x => x.Opacity).To(1).In(Duration));
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(AssociatedObject));
        }
    }
}