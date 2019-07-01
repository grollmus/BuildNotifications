using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BuildNotifications.Resources.Animation;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.GroupDefinitionSelection
{
    internal class GroupDefinitionButtonToggle : TweenTriggerAction<Button>
    {
        public bool DoHide { get; set; }

        protected override void Invoke(object parameter)
        {
            var targetColorKey = DoHide ? "Background3" : "Foreground1";
            var targetColorBrush = (SolidColorBrush) AssociatedObject.TryFindResource(targetColorKey);
            var targetColor = targetColorBrush.Color;

            var scaleTransform = new ScaleTransform(DoHide ? 1.2 : 1.0, DoHide ? 1.2 : 1.0, Anchor.Position(AssociatedObject).X, Anchor.Position(AssociatedObject).Y);
            var rotateTransform = new RotateTransform(DoHide ? 90 : 0, Anchor.Position(AssociatedObject).X, Anchor.Position(AssociatedObject).Y);
            var group = new TransformGroup();
            group.Children = new TransformCollection(new Transform[] {scaleTransform, rotateTransform});
            AssociatedObject.RenderTransform = group;

            var counterScaleTransform = new ScaleTransform(DoHide ? 0.8 : 1.0, DoHide ? 0.8 : 1.0, Anchor.Position(AssociatedObject.Content as UIElement).X, Anchor.Position(AssociatedObject.Content as UIElement).Y);
            var counterRotateTransform = new RotateTransform(DoHide ? -90 : 0, Anchor.Position(AssociatedObject.Content as UIElement).X, Anchor.Position(AssociatedObject.Content as UIElement).Y);
            var counterGroup = new TransformGroup();
            counterGroup.Children = new TransformCollection(new Transform[] {counterScaleTransform, counterRotateTransform});
            ((UIElement) AssociatedObject.Content).RenderTransform = counterGroup;

            var existingBrush = AssociatedObject.Foreground as SolidColorBrush;
            var brush = new SolidColorBrush(existingBrush?.Color ?? Colors.White);

            AssociatedObject.Foreground = brush;

            var tweens = new List<Timeline>();
            tweens.Add(brush.Tween(x => x.Color, ColorTween.ColorProgressFunction).To(targetColor).In(Duration));
            tweens.Add(scaleTransform.Tween(x => x.ScaleX).To(DoHide ? 1.0 : 1.2).In(Duration).Ease(Easing.ExpoEaseOut));
            tweens.Add(scaleTransform.Tween(x => x.ScaleY).To(DoHide ? 1.0 : 1.2).In(Duration).Ease(Easing.ExpoEaseOut));
            tweens.Add(rotateTransform.Tween(x => x.Angle).To(DoHide ? 0 : 90).In(Duration).Ease(Easing.ExpoEaseOut));

            tweens.Add(counterScaleTransform.Tween(x => x.ScaleX).To(DoHide ? 1.0 : 0.8).In(Duration).Ease(Easing.ExpoEaseOut));
            tweens.Add(counterScaleTransform.Tween(x => x.ScaleY).To(DoHide ? 1.0 : 0.8).In(Duration).Ease(Easing.ExpoEaseOut));
            tweens.Add(counterRotateTransform.Tween(x => x.Angle).To(DoHide ? 0 : -90).In(Duration).Ease(Easing.ExpoEaseOut));

            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);
            globalTweenHandler.Add(tweens.ToSequenceWithTarget(AssociatedObject));
        }
    }
}