using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal class BuildFadeOut : TriggerAction<FrameworkElement>
    {
        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            var size = AssociatedObject.RenderSize;
            var scaleTransform = new ScaleTransform(1.0, 1.0, 0, size.Height / 2);

            var tweens = new List<Timeline>
            {
                scaleTransform.Tween(x => x.ScaleX).And(x => x.ScaleY).To(0.5).In(0.15).Ease(Easing.ExpoEaseIn),
                AssociatedObject.Tween(x => x.Opacity).To(0).In(0.15)
            };
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(AssociatedObject));

            AssociatedObject.RenderTransform = scaleTransform;
        }
    }
}