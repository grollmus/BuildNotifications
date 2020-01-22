using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media.Effects;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation
{
    internal class Blur : TweenTriggerAction<UIElement>
    {
        public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(
            "Amount", typeof(double), typeof(Blur), new PropertyMetadata(default(double)));

        public double Amount
        {
            get => (double) GetValue(AmountProperty);
            set => SetValue(AmountProperty, value);
        }

        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;

            if (!(AssociatedObject.Effect is BlurEffect blurEffect))
            {
                blurEffect = new BlurEffect();
                AssociatedObject.Effect = blurEffect;
            }

            globalTweenHandler.ClearTweensOf(blurEffect);

            globalTweenHandler.Add(blurEffect.Tween(e => e.Radius).To(Amount).In(Duration).Ease(Easing.ExpoEaseOut));
        }
    }
}