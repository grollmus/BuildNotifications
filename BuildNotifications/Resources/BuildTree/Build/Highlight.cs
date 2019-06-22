using System.Windows.Interactivity;
using System.Windows.Media;
using System.Windows.Shapes;
using BuildNotifications.Resources.Animation;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.Build
{
    internal class Highlight : TriggerAction<Rectangle>
    {
        public bool DoHighlight { get; set; }

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            var brush = new SolidColorBrush();

            if (DoHighlight)
            {
                var targetBrush = AssociatedObject.FindResource("Background3") as SolidColorBrush;
                var targetColor = targetBrush?.Color ?? Colors.White;

                brush.Color = Colors.White;
                AssociatedObject.Fill = brush;

                var highlightTween = brush.Tween(x => x.Color, ColorTween.ColorProgressFunction)
                    .To(targetColor).In(2).Ease(Easing.ExpoEaseIn);
                var opacityTween = AssociatedObject.Tween(x => x.Opacity).To(1).In(0.1);

                globalTweenHandler.Add(new SequenceOfTarget(AssociatedObject, highlightTween, opacityTween));
            }
            else
            {
                globalTweenHandler.Add(AssociatedObject.Tween(x => x.Opacity).To(0).In(0.25));
            }
        }
    }
}