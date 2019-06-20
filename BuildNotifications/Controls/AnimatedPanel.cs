using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace BuildNotifications.Controls
{
    public class AnimatedPanel : Panel
    {
        private readonly ILayoutStrategy _strategy = new TableLayoutStrategy();

        protected override Size MeasureOverride(Size availableSize)
        {
            InitializeEmptyOrder();

            var measures = MeasureChildren();

            _strategy.Calculate(availableSize, measures);

            var index = -1;
            foreach (var child in Children.OfType<UIElement>().OrderBy(GetOrder))
            {
                index++;
                SetDesiredPosition(child, _strategy.GetPosition(index));
            }

            return _strategy.ResultSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (var child in Children.OfType<UIElement>().OrderBy(GetOrder))
            {
                var position = GetPosition(child);
                if (double.IsNaN(position.Top))
                    position = GetDesiredPosition(child);
                child.Arrange(position);
            }

            return _strategy.ResultSize;
        }

        private Size[] MeasureChildren()
        {
            if (_measures == null || Children.Count != _measures.Length)
            {
                _measures = new Size[Children.Count];

                var infinity = new Size(Double.PositiveInfinity, Double.PositiveInfinity);

                foreach (UIElement child in Children)
                {
                    child.Measure(infinity);
                }

                var i = 0;
                foreach (var measure in Children.OfType<UIElement>().OrderBy(GetOrder).Select(ch => ch.DesiredSize))
                {
                    _measures[i] = measure;
                    i++;
                }
            }

            return _measures;
        }

        private void InitializeEmptyOrder()
        {
            var next = Children.OfType<UIElement>().Max(GetOrder) + 1;
            foreach (var child in Children.OfType<UIElement>().Where(child => GetOrder(child) == -1))
            {
                SetOrder(child, next);
                next++;
            }
        }

        public static readonly DependencyProperty OrderProperty;
        public static readonly DependencyProperty PositionProperty;
        public static readonly DependencyProperty DesiredPositionProperty;
        private Size[] _measures;

        static AnimatedPanel()
        {
            PositionProperty = DependencyProperty.RegisterAttached(
                "Position",
                typeof(Rect),
                typeof(AnimatedPanel),
                new FrameworkPropertyMetadata(
                    new Rect(double.NaN, double.NaN, double.NaN, double.NaN),
                    FrameworkPropertyMetadataOptions.AffectsParentArrange));

            DesiredPositionProperty = DependencyProperty.RegisterAttached(
                "DesiredPosition",
                typeof(Rect),
                typeof(AnimatedPanel),
                new FrameworkPropertyMetadata(
                    new Rect(double.NaN, double.NaN, double.NaN, double.NaN),
                    OnDesiredPositionChanged));

            OrderProperty = DependencyProperty.RegisterAttached(
                "Order",
                typeof(int),
                typeof(AnimatedPanel),
                new FrameworkPropertyMetadata(
                    -1,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure));
        }

        private static void OnDesiredPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var desiredPosition = (Rect)e.NewValue;
            AnimateToPosition(d, desiredPosition);
        }

        private static void AnimateToPosition(DependencyObject d, Rect desiredPosition)
        {
            var position = GetPosition(d);
            if (double.IsNaN(position.X))
            {
                SetPosition(d, desiredPosition);
                return;
            }

            var distance = Math.Max(
                (desiredPosition.TopLeft - position.TopLeft).Length,
                (desiredPosition.BottomRight - position.BottomRight).Length);

            var animationTime = TimeSpan.FromMilliseconds(distance * 2);
            var animation = new RectAnimation(position, desiredPosition, new Duration(animationTime))
            {
                DecelerationRatio = 1
            };
            ((UIElement)d).BeginAnimation(PositionProperty, animation);
        }

        public static int GetOrder(DependencyObject obj)
        {
            return (int)obj.GetValue(OrderProperty);
        }

        public static void SetOrder(DependencyObject obj, int value)
        {
            obj.SetValue(OrderProperty, value);
        }

        public static Rect GetPosition(DependencyObject obj)
        {
            return (Rect)obj.GetValue(PositionProperty);
        }

        public static void SetPosition(DependencyObject obj, Rect value)
        {
            obj.SetValue(PositionProperty, value);
        }

        public static Rect GetDesiredPosition(DependencyObject obj)
        {
            return (Rect)obj.GetValue(DesiredPositionProperty);
        }

        public static void SetDesiredPosition(DependencyObject obj, Rect value)
        {
            obj.SetValue(DesiredPositionProperty, value);
        }
    }
}
