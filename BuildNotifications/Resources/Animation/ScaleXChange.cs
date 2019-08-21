using System.Windows;
using System.Windows.Media;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation
{
    internal class ScaleXChange : TweenTriggerAction<FrameworkElement>
    {
        public ScaleXChange()
        {
            Duration = 0.35;
        }

        public double Delay { get; set; }

        public static readonly DependencyProperty TargetScaleXProperty = DependencyProperty.Register(
            "TargetScaleX", typeof(double), typeof(ScaleXChange), new PropertyMetadata(1.0));

        public double TargetScaleX
        {
            get => (double) GetValue(TargetScaleXProperty);
            set => SetValue(TargetScaleXProperty, value);
        }

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;

            ScaleTransform scaleTransform;
            if (TargetElement.RenderTransform is ScaleTransform existingTransform)
            {
                scaleTransform = existingTransform;
                globalTweenHandler.ClearTweensOf(existingTransform);
            }
            else
                scaleTransform = new ScaleTransform(1, 0);

            globalTweenHandler.Add(scaleTransform.Tween(x => x.ScaleX).To(TargetScaleX).In(Duration).Delay(Delay).Ease(Easing.QuadraticEaseOut));
        }
    }
}