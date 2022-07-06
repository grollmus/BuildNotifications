using System.Windows;
using System.Windows.Media.Effects;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.Animation;

internal class Blur : TweenTriggerAction<UIElement>
{
    public double Amount
    {
        get => (double)GetValue(AmountProperty);
        set => SetValue(AmountProperty, value);
    }

    protected override void Invoke(object parameter)
    {
        var globalTweenHandler = App.GlobalTweenHandler;

        if (AssociatedObject.Effect is not BlurEffect blurEffect)
        {
            blurEffect = new BlurEffect();
            AssociatedObject.Effect = blurEffect;
        }

        globalTweenHandler.ClearTweensOf(blurEffect);

        globalTweenHandler.Add(blurEffect.Tween(e => e.Radius).To(Amount).In(Duration).Ease(Easing.ExpoEaseOut).OnComplete((_, _) =>
        {
            // remove the effect, as even an amount of zero causes a big performance hit even though the effect is invisible
            if (Amount <= 0)
                AssociatedObject.Effect = null;
        }));
    }

    public static readonly DependencyProperty AmountProperty = DependencyProperty.Register(
        "Amount", typeof(double), typeof(Blur), new PropertyMetadata(default(double)));
}