using System.Collections.Generic;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Media;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.Build
{
    internal class BuildFadeIn : TriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            if (AlreadyFadedInElements.Contains(AssociatedObject))
                return;
            AlreadyFadedInElements.Add(AssociatedObject);
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            var scaleTransform = new ScaleTransform(0, 1.0, 0, 0);
            globalTweenHandler.Add(scaleTransform.Tween(x => x.ScaleX).To(1.0).In(2).Ease(Easing.QuadraticEaseInOut));

            AssociatedObject.RenderTransform = scaleTransform;
        }

        private static readonly HashSet<UIElement> AlreadyFadedInElements = new HashSet<UIElement>();
    }
}