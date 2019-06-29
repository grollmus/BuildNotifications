using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal class ElementFadeOut : TweenTriggerAction
    {
        public ElementFadeOut()
        {
            Anchor = Anchor.MiddleLeft;
            Duration = 0.15;
        }

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            var tweens = new List<Timeline>();

            if (DoScale)
            {
                var scaleTransform = new ScaleTransform(1.0, 1.0, Anchor.Position(AssociatedObject).X, Anchor.Position(AssociatedObject).Y);
                tweens.Add(scaleTransform.Tween(x => x.ScaleX).And(x => x.ScaleY).To(0.5).In(Duration).Ease(Easing.ExpoEaseIn));
                AssociatedObject.RenderTransform = scaleTransform;
            }

            tweens.Add(AssociatedObject.Tween(x => x.Opacity).To(0).In(Duration));
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(AssociatedObject).OnComplete((sender, objects) => { AssociatedObject.Visibility = Visibility.Hidden; }));
        }
    }
}