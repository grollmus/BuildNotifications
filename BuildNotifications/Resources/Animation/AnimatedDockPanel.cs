using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation
{
    internal class AnimatedDockPanel : DockPanel
    {
        public double AnimationDuration { get; set; } = 0.4;

        protected override Size ArrangeOverride(Size finalSize)
        {
            var positions = new List<Point>();
            foreach (UIElement child in Children)
            {
                var oldPos = child.TranslatePoint(new Point(0, 0), this);
                if (child.RenderTransform is TranslateTransform translateTransform)
                {
                    oldPos.X += translateTransform.X;
                    oldPos.Y += translateTransform.Y;
                }

                positions.Add(oldPos);
            }

            var arrangedSize = base.ArrangeOverride(finalSize);

            var index = 0;
            var origin = new Point(0, 0);
            foreach (UIElement child in Children)
            {
                var oldPos = positions[index];
                var newPos = child.TranslatePoint(new Point(0, 0), this);

                if (!oldPos.Equals(newPos))
                {
                    if (oldPos.Equals(origin))
                    {
                        oldPos = new Point(newPos.X + 30, newPos.Y);
                    }

                    var tween = CreateTween(oldPos, newPos, child);
                    var globalTweenHandler = App.GlobalTweenHandler;
                    globalTweenHandler.ClearTweensOf(child);
                    globalTweenHandler.Add(tween);
                }

                index++;
            }

            return arrangedSize;
        }

        private Timeline CreateTween(Point oldPos, Point newPos, UIElement child)
        {
            var deltaX = oldPos.X - newPos.X;
            var deltaY = oldPos.Y - newPos.Y;

            var layoutTransform = new TranslateTransform(deltaX, deltaY);
            child.RenderTransform = layoutTransform;

            var seq = new SequenceOfTarget(child, layoutTransform.Tween(x => x.X).And(x => x.Y).To(0).In(AnimationDuration).Delay(0.2).Ease(Easing.ExpoEaseOut));

            return seq;
        }
    }
}