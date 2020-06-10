using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;
using BuildNotifications.Resources.Animation;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.Build
{
    internal class ShortHighlight : TriggerAction<Rectangle>
    {
        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            // do nothing when animations are disabled
            if (globalTweenHandler.TimeModifier > 10)
                return;

            globalTweenHandler.ClearTweensOf(AssociatedObject);

            AssociatedObject.Visibility = Visibility.Visible;

            var brush = new SolidColorBrush();
            var targetBrush = new SolidColorBrush(Colors.Transparent);
            var background = AssociatedObject.FindResource("Background1") as SolidColorBrush;
            var initialHighlightBrush = AssociatedObject.FindResource("Foreground1") as SolidColorBrush;
            var targetColor = targetBrush.Color;
            var initialColor = initialHighlightBrush?.Color ?? Colors.White;

            brush.Color = background?.Color ?? new Color();
            AssociatedObject.Fill = brush;

            const double initialTime = 0.5;

            var toInitialColor = brush.Tween(x => x.Color, ColorTween.ColorProgressFunction)
                .To(initialColor).In(initialTime).Ease(Easing.QuinticEaseIn);

            var highlightTween = brush.Tween(x => x.Color, ColorTween.ColorProgressFunction)
                .To(targetColor).In(0.5).Ease(Easing.QuinticEaseOut).Delay(initialTime + 0.1);
            var opacityTween = AssociatedObject.Tween(x => x.Opacity).To(1).In(0);

            globalTweenHandler.Add(new SequenceOfTarget(AssociatedObject, highlightTween, opacityTween, toInitialColor));
        }
    }
}
