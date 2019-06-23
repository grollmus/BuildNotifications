using System.Collections.Generic;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal class ElementFadeOutScaleX : TweenTriggerAction
    {
        public ElementFadeOutScaleX()
        {
            Anchor = Anchor.MiddleLeft;
        }

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            var tweens = new List<Timeline>();
            if (DoScale)
            {
                var scaleTransform = new ScaleTransform(1.0, 1.0, Anchor.Position(AssociatedObject).Width, Anchor.Position(AssociatedObject).Height);
                tweens.Add(scaleTransform.Tween(x => x.ScaleX).To(0.0).In(Duration).Ease(Easing.QuadraticEaseInOut));
                AssociatedObject.RenderTransform = scaleTransform;
            }

            tweens.Add(AssociatedObject.Tween(x => x.Opacity).To(0).In(Duration));
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(AssociatedObject));
        }
    }
}