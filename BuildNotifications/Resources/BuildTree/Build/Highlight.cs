using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;
using BuildNotifications.Resources.Animation;
using TweenSharp;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.Build
{
    internal class Highlight : TriggerAction<Rectangle>
    {
        public bool DoHighlight { get; set; }

        public bool DoLongHighlight { get; set; } = true;

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            var brush = new SolidColorBrush();

            if (AssociatedObject.Visibility != Visibility.Visible)
                AssociatedObject.Visibility = Visibility.Visible;

            if (DoHighlight)
            {
                AssociatedObject.Visibility = Visibility.Visible;
                if (DoLongHighlight)
                    LongHighlight(brush, globalTweenHandler);
                else
                    FastHighlight(brush, globalTweenHandler);
            }
            else
                globalTweenHandler.Add(AssociatedObject.Tween(x => x.Opacity).To(0).In(0.25));
        }

        private void FastHighlight(SolidColorBrush brush, TweenHandler globalTweenHandler)
        {
            var targetBrush = AssociatedObject.FindResource("Background3") as SolidColorBrush;
            var background = AssociatedObject.FindResource("Background1") as SolidColorBrush;
            var initialHighlightBrush = AssociatedObject.FindResource("Foreground1") as SolidColorBrush;
            var targetColor = targetBrush?.Color ?? Colors.White;
            var initialColor = initialHighlightBrush?.Color ?? Colors.White;

            brush.Color = background?.Color ?? new Color();
            AssociatedObject.Fill = brush;

            var initialTime = 0.5;

            var toInitialColor = brush.Tween(x => x.Color, ColorTween.ColorProgressFunction)
                .To(initialColor).In(initialTime).Ease(Easing.QuinticEaseIn);

            var highlightTween = brush.Tween(x => x.Color, ColorTween.ColorProgressFunction)
                .To(targetColor).In(0.5).Ease(Easing.QuinticEaseOut).Delay(initialTime + 0.1);
            var opacityTween = AssociatedObject.Tween(x => x.Opacity).To(1).In(0);

            globalTweenHandler.Add(new SequenceOfTarget(AssociatedObject, highlightTween, opacityTween, toInitialColor));
        }

        private void LongHighlight(SolidColorBrush brush, TweenHandler globalTweenHandler)
        {
            var targetBrush = AssociatedObject.FindResource("Background3") as SolidColorBrush;
            var initialHighlightBrush = AssociatedObject.FindResource("Foreground1") as SolidColorBrush;
            var targetColor = targetBrush?.Color ?? Colors.White;
            var initialColor = initialHighlightBrush?.Color ?? Colors.White;

            brush.Color = initialColor;
            AssociatedObject.Fill = brush;

            var highlightTween = brush.Tween(x => x.Color, ColorTween.ColorProgressFunction)
                .To(targetColor).In(2).Ease(Easing.ExpoEaseIn);
            var opacityTween = AssociatedObject.Tween(x => x.Opacity).To(1).In(0.1);

            globalTweenHandler.Add(new SequenceOfTarget(AssociatedObject, highlightTween, opacityTween));
        }
    }
}