using System.Windows;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation
{
    internal class WidthChange : TweenTriggerAction<FrameworkElement>
    {
        public WidthChange()
        {
            Duration = 0.35;
        }

        public double Delay { get; set; }

        public bool IsActive
        {
            get => (bool) GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }

        public double TargetWidth
        {
            get => (double) GetValue(TargetWidthProperty);
            set => SetValue(TargetWidthProperty, value);
        }

        protected override void Invoke(object parameter)
        {
            if (!IsActive)
                return;
            var globalTweenHandler = App.GlobalTweenHandler;

            globalTweenHandler.ClearTweensOf(TargetElement);

            globalTweenHandler.Add(TargetElement.Tween(x => x.Width).To(TargetWidth).In(Duration).Delay(Delay).Ease(Easing.QuadraticEaseOut));
        }

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
            "IsActive", typeof(bool), typeof(WidthChange), new PropertyMetadata(true));

        public static readonly DependencyProperty TargetWidthProperty = DependencyProperty.Register(
            "TargetWidth", typeof(double), typeof(WidthChange), new PropertyMetadata(default(double)));
    }
}