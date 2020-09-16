using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BuildNotifications.ViewModel.Utils;
using TweenSharp;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation
{
    internal class AnimatedPanelHelper
    {
        public double AnimationDuration { get; set; } = 0.4;

        public double Delay { get; set; }

        public void AnimateToNewPositions(Dictionary<UIElement, Point> positions, Panel panel)
        {
            var origin = new Point(0, 0);
            var globalTweenHandler = App.GlobalTweenHandler;
            foreach (var child in panel.Children.Enumerate())
            {
                if (!positions.ContainsKey(child))
                    continue;

                var oldPos = positions[child];
                var newPos = child.TranslatePoint(new Point(0, 0), panel);

                if (oldPos.Equals(newPos))
                    continue;

                if (oldPos.Equals(origin))
                    continue;

                CreateTween(oldPos, newPos, child, globalTweenHandler);
            }
        }

        public void CreateTween(Point oldPos, Point newPos, UIElement child, TweenHandler tweenHandler)
        {
            var deltaX = oldPos.X - newPos.X;
            var deltaY = oldPos.Y - newPos.Y;

            tweenHandler.ClearTweensOf(child);

            if (child.RenderTransform is TranslateTransform translateTransform)
            {
                translateTransform.X += deltaX;
                translateTransform.Y += deltaY;
            }
            else
            {
                translateTransform = new TranslateTransform(deltaX, deltaY);
                child.RenderTransform = translateTransform;
            }

            var xTween = translateTransform.Tween(x => x.X)
                .To(0)
                .In(AnimationDuration)
                .Ease(Easing.ExpoEaseOut)
                .Delay(Delay);

            var yTween = translateTransform.Tween(x => x.Y)
                .To(0)
                .In(AnimationDuration)
                .Ease(Easing.ExpoEaseOut)
                .Delay(Delay);

            tweenHandler.Add(new SequenceOfTarget(child, xTween, yTween));
        }

        public Dictionary<UIElement, Point> StoreOldPositions(Panel panel)
        {
            var positions = new Dictionary<UIElement, Point>();
            foreach (var child in panel.Children.Enumerate())
            {
                if (child == null)
                    continue;

                var oldPos = child.TranslatePoint(new Point(0, 0), panel);
                positions.Add(child, oldPos);
            }

            return positions;
        }
    }
}