using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using BuildNotifications.Resources.Animation;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Settings
{
    internal class TextChangedAnimation : TweenTriggerAction<TextBlock>
    {
        protected override void Invoke(object parameter)
        {
            var targetColorKey = "Foreground1HalfTransparency";
            var targetColorBrush = (SolidColorBrush) AssociatedObject.TryFindResource(targetColorKey);
            var targetColor = targetColorBrush.Color;

            var scaleTransform = new ScaleTransform(0.9, 0.9, Anchor.Position(AssociatedObject).X, Anchor.Position(AssociatedObject).Y);
            AssociatedObject.RenderTransform = scaleTransform;

            var brush = new SolidColorBrush(Colors.White);

            AssociatedObject.Foreground = brush;

            var tweens = new List<Timeline>
            {
                brush.Tween(x => x.Color, ColorTween.ColorProgressFunction).To(targetColor).In(Duration),
                scaleTransform.Tween(x => x.ScaleX).To(1).In(Duration).Ease(Easing.ExpoEaseOut),
                scaleTransform.Tween(x => x.ScaleY).To(1).In(Duration).Ease(Easing.ExpoEaseOut)
            };

            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(AssociatedObject));
        }
    }
}