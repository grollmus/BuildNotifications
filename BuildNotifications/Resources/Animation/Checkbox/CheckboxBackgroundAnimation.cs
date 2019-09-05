using System.Windows.Media;
using System.Windows.Shapes;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation.Checkbox
{
    internal class CheckboxBackgroundAnimation : TweenTriggerAction<Rectangle>
    {
        public bool IsChecked { get; set; }

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;

            var targetBrush = IsChecked ? CheckboxSelectedToBackgroundColorConverter.CheckedBrush : CheckboxSelectedToBackgroundColorConverter.UncheckedBrush;
            var targetColor = targetBrush.Color;

            globalTweenHandler.ClearTweensOf(TargetElement);

            var existingBrush = TargetElement.Fill as SolidColorBrush;
            if (existingBrush == null || existingBrush.IsFrozen)
            {
                existingBrush = new SolidColorBrush();
                TargetElement.Fill = existingBrush;
            }

            var brushTween = existingBrush.Tween(x => x.Color, ColorTween.ColorProgressFunction).To(targetColor).In(Duration).Ease(Easing.QuadraticEaseOut);

            globalTweenHandler.Add(new SequenceOfTarget(TargetElement, brushTween));
        }
    }
}