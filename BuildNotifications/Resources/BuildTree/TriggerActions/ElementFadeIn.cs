using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal class ElementFadeIn : TriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            var size = AssociatedObject.RenderSize;
            var scaleTransform = new ScaleTransform(0.5, 0.5, size.Width / 2, size.Height / 2);

            var tweens = new List<Timeline>
            {
                scaleTransform.Tween(x => x.ScaleX).And(x => x.ScaleY).To(1.0).In(1).Ease(Easing.BackEaseOut),
                AssociatedObject.Tween(x => x.Opacity).From(0).In(1)
            };
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(AssociatedObject));

            AssociatedObject.RenderTransform = scaleTransform;
        }
    }
}