using System.Collections.Generic;
using System.Windows.Media;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Icons
{
    internal class ButtonInvokedAnimation : TweenTriggerAction<IconButton>
    {
        public ButtonInvokedAnimation()
        {
            Duration = 0.2;
        }

        public int Repeat { get; set; } = 0;

        protected override void Invoke(object parameter)
        {
            var scaleTransform = new ScaleTransform(1.25, 1.25, Anchor.Position(AssociatedObject).X, Anchor.Position(AssociatedObject).Y);
            var rotateTransform = new RotateTransform(7, Anchor.Position(AssociatedObject).X, Anchor.Position(AssociatedObject).Y);
            var group = new TransformGroup();
            group.Children = new TransformCollection(new Transform[] {scaleTransform, rotateTransform});
            AssociatedObject.RenderTransform = group;

            var tweens = new List<Timeline>();
            tweens.Add(scaleTransform.Tween(x => x.ScaleX).To(1.0).In(Duration).Ease(Easing.QuadraticEaseIn).Repeat(Repeat));
            tweens.Add(scaleTransform.Tween(x => x.ScaleY).To(1.0).In(Duration).Ease(Easing.QuadraticEaseIn).Repeat(Repeat));
            tweens.Add(rotateTransform.Tween(x => x.Angle).To(0).In(Duration).Ease(Easing.QuadraticEaseIn));

            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(AssociatedObject));
        }
    }
}