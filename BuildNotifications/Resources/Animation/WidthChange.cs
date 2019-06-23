using System.Windows;
using System.Windows.Interactivity;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation
{
    internal class WidthChange : TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty TargetWidthProperty = DependencyProperty.Register(
            "TargetWidth", typeof(double), typeof(WidthChange), new PropertyMetadata(default(double)));

        public double TargetWidth
        {
            get => (double) GetValue(TargetWidthProperty);
            set => SetValue(TargetWidthProperty, value);
        }

        public double Duration { get; set; } = 0.35;

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            globalTweenHandler.Add(AssociatedObject.Tween(x => x.Width).To(TargetWidth).In(Duration).Ease(Easing.QuadraticEaseOut));
        }
    }
}
