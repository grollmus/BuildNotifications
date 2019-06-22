using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Controls
{
    internal class AnimatedWrapPanel : WrapPanel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            var positions = new List<Point>();
            foreach (UIElement child in Children)
            {
                positions.Add(child.TranslatePoint(new Point(0, 0), this));
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

            return layoutTransform.Tween(x => x.X).And(x => x.Y).To(0).In(0.4).Delay(0.2).Ease(Easing.ExpoEaseOut);
        }
    }
}