using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TweenSharp;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation
{
    internal class AnimatedPanelHelper
    {
        public Dictionary<UIElement, Point> StoreOldPositions(Panel panel)
        {
            var positions = new Dictionary<UIElement, Point>();
            foreach (UIElement child in panel.Children)
            {
                var oldPos = child.TranslatePoint(new Point(0, 0), panel);
                positions.Add(child, oldPos);
            }

            return positions;
        }

        public void AnimateToNewPositions(Dictionary<UIElement, Point> positions, Panel panel, double animationDuration)
        {
            var origin = new Point(0, 0);
            var globalTweenHandler = App.GlobalTweenHandler;
            foreach (UIElement child in panel.Children)
            {
                if (!positions.ContainsKey(child))
                    continue;

                var oldPos = positions[child];
                var newPos = child.TranslatePoint(new Point(0, 0), panel);

                if (oldPos.Equals(newPos))
                    continue;

                if (oldPos.Equals(origin))
                {
                    oldPos = new Point(newPos.X + 30, newPos.Y);
                }

                CreateTween(oldPos, newPos, child, animationDuration, globalTweenHandler);
            }
        }

        public void CreateTween(Point oldPos, Point newPos, UIElement child, double animationDuration, TweenHandler tweenHandler)
        {
            var deltaX = oldPos.X - newPos.X;
            var deltaY = oldPos.Y - newPos.Y;

            //if (IsOutsideWindowBounds(child))
            //{
            //    return;
            //}

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
                .In(animationDuration)
                .Ease(Easing.ExpoEaseOut);

            var yTween = translateTransform.Tween(x => x.Y)
                .To(0)
                .In(animationDuration)
                .Ease(Easing.ExpoEaseOut);

            tweenHandler.Add(new SequenceOfTarget(child, xTween, yTween));
        }

        private bool IsOutsideWindowBounds(UIElement element)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null)
                return false;

            var pos = element.TranslatePoint(new Point(0, 0), mainWindow);
            return (pos.X + element.RenderSize.Width < 0) || (pos.Y + element.RenderSize.Height < 0) || (pos.X >= mainWindow.ActualWidth) || (pos.Y >= mainWindow.ActualHeight);
        }
    }
}