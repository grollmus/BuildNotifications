using System.Windows;
using System.Windows.Media;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation
{
    internal class BrushColorChange : TweenTriggerAction<FrameworkElement>
    {
        public BrushColorChange()
        {
            Duration = 0.35;
        }

        public double Delay { get; set; }

        public static readonly DependencyProperty TargetDependencyPropertyProperty = DependencyProperty.Register(
            "TargetDependencyProperty", typeof(DependencyProperty), typeof(BrushColorChange), new PropertyMetadata(default(DependencyProperty)));

        public DependencyProperty TargetDependencyProperty
        {
            get { return (DependencyProperty) GetValue(TargetDependencyPropertyProperty); }
            set { SetValue(TargetDependencyPropertyProperty, value); }
        }

        public static readonly DependencyProperty TargetColorProperty = DependencyProperty.Register(
            "TargetColor", typeof(SolidColorBrush), typeof(BrushColorChange), new PropertyMetadata(default(SolidColorBrush)));

        public SolidColorBrush TargetColor
        {
            get { return (SolidColorBrush) GetValue(TargetColorProperty); }
            set { SetValue(TargetColorProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;

            globalTweenHandler.ClearTweensOf(TargetElement);

            var targetBrush = TargetElement.GetValue(TargetDependencyProperty) as SolidColorBrush;
            if (targetBrush == null || targetBrush.IsFrozen)
            {
                targetBrush = new SolidColorBrush();
                TargetElement.SetValue(TargetDependencyProperty, targetBrush);
            }

            var brushTween = targetBrush.Tween(x => x.Color, ColorTween.ColorProgressFunction).To(TargetColor.Color).In(Duration).Delay(Delay).Ease(Easing.QuadraticEaseOut);

            globalTweenHandler.Add(new SequenceOfTarget(TargetElement, brushTween));
        }
    }
}