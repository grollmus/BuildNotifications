using System.Windows;
using System.Windows.Controls;
using BuildNotifications.Resources.Animation;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Text
{
    internal class FontSizeChange : TweenTriggerAction<FrameworkElement>
    {
        public FontSizeChange()
        {
            Duration = 0.35;
        }

        public double Delay { get; set; }

        public double TargetSize
        {
            get => (double) GetValue(TargetSizeProperty);
            set => SetValue(TargetSizeProperty, value);
        }

        protected override void Invoke(object parameter)
        {
            if (!(TargetElement is TextBlock textBlock))
                return;

            var globalTweenHandler = App.GlobalTweenHandler;

            globalTweenHandler.ClearTweensOf(textBlock);

            globalTweenHandler.Add(textBlock.Tween(x => x.FontSize).To(TargetSize).In(Duration).Delay(Delay).Ease(Easing.QuadraticEaseOut));
        }

        public static readonly DependencyProperty TargetSizeProperty = DependencyProperty.Register(
            "TargetSize", typeof(double), typeof(FontSizeChange), new PropertyMetadata(default(double)));
    }
}