using System.Windows;
using System.Windows.Interactivity;

namespace BuildNotifications.Resources.BuildTree.TriggerActions
{
    internal abstract class TweenTriggerAction<T> : TriggerAction<T> where T : DependencyObject
    {
        public double Duration { get; set; } = 1;

        public Anchor Anchor { get; set; } = Anchor.Center;

        public bool DoScale { get; set; } = true;

        public static readonly DependencyProperty TargetProperty = DependencyProperty.Register(
            "Target", typeof(T), typeof(TweenTriggerAction<T>), new PropertyMetadata(default(T)));

        public T Target
        {
            get => (T) GetValue(TargetProperty);
            set => SetValue(TargetProperty, value);
        }

        protected T TargetElement => Target ?? AssociatedObject;
    }
}