using System.Windows;
using System.Windows.Media;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.GroupDefinitionSelection;

internal class GroupDefinitionFadeIn : TweenTriggerAction<UIElement>
{
    protected override void Invoke(object parameter)
    {
        var globalTweenHandler = App.GlobalTweenHandler;
        globalTweenHandler.ClearTweensOf(TargetElement);

        var anchor = Anchor.Position(TargetElement);
        var scaleTransform = new ScaleTransform(0.2, 0.7, anchor.X, anchor.Y);
        TargetElement.Opacity = 0;
        globalTweenHandler.Add(scaleTransform.Tween(x => x.ScaleX).To(1.0).In(Duration).Ease(Easing.ExpoEaseInOut));
        globalTweenHandler.Add(scaleTransform.Tween(x => x.ScaleY).To(1.0).In(Duration).Ease(Easing.ExpoEaseInOut));
        globalTweenHandler.Add(TargetElement.Tween(x => x.Opacity).To(1.0).In(Duration).Ease(Easing.ExpoEaseInOut));

        TargetElement.RenderTransform = scaleTransform;
    }
}