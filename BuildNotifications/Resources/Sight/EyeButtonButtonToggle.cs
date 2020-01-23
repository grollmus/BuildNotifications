using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BuildNotifications.Resources.Animation;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Sight
{
    internal class EyeButtonButtonToggle : TweenTriggerAction<Button>
    {
        public bool DoHide { get; set; }

        protected override void Invoke(object parameter)
        {
            if (!(AssociatedObject.Content is UIElement buttonContent))
                return;

            var targetColorKey = DoHide ? "Background3" : "Foreground1";
            var targetColorBrush = (SolidColorBrush) AssociatedObject.TryFindResource(targetColorKey);
            var targetColor = targetColorBrush.Color;

            const double hiddenScale = 1.0;
            const double shownScale = 1.3;

            var scaleTransform = new ScaleTransform(1.0, DoHide ? shownScale : hiddenScale, Anchor.Position(AssociatedObject).X, Anchor.Position(AssociatedObject).Y);
            AssociatedObject.RenderTransform = scaleTransform;

            var counterScaleTransform = new ScaleTransform(1.0, DoHide ? 1 / shownScale : 1 / hiddenScale, Anchor.Position(buttonContent).X, Anchor.Position(buttonContent).Y);

            ((UIElement) AssociatedObject.Content).RenderTransform = counterScaleTransform;

            var existingBrush = AssociatedObject.Foreground as SolidColorBrush;
            var brush = new SolidColorBrush(existingBrush?.Color ?? Colors.White);

            AssociatedObject.Foreground = brush;

            var tweens = new List<Timeline>
            {
                brush.Tween(x => x.Color, ColorTween.ColorProgressFunction).To(targetColor).In(Duration),
                scaleTransform.Tween(x => x.ScaleY).To(DoHide ? hiddenScale : shownScale).In(Duration).Ease(Easing.ExpoEaseOut),
                counterScaleTransform.Tween(x => x.ScaleY).To(DoHide ? 1 / hiddenScale : 1 / shownScale).In(Duration).Ease(Easing.ExpoEaseOut),
            };

            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(AssociatedObject));
        }
    }
}