using System.Windows.Media;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.GroupDefinitionSelection
{
    internal class GroupDefinitionFadeIn : TweenTriggerAction
    {
        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            var anchor = Anchor.Position(AssociatedObject);
            var scaleTransform = new ScaleTransform(0.25, 0.25, anchor.X, anchor.Y);
            AssociatedObject.Opacity = 0;
            globalTweenHandler.Add(scaleTransform.Tween(x => x.ScaleX).To(1.0).In(Duration).Ease(Easing.BackEaseOut));
            globalTweenHandler.Add(scaleTransform.Tween(x => x.ScaleY).To(1.0).In(Duration).Ease(Easing.BackEaseOut));
            globalTweenHandler.Add(AssociatedObject.Tween(x => x.Opacity).To(1.0).In(Duration).Ease(Easing.QuadraticEaseInOut));

            AssociatedObject.RenderTransform = scaleTransform;
        }
    }
}