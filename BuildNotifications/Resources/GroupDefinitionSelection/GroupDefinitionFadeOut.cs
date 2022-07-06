using System.Windows;
using System.Windows.Media;
using BuildNotifications.Resources.BuildTree.TriggerActions;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.GroupDefinitionSelection;

internal class GroupDefinitionFadeOut : TweenTriggerAction<UIElement>
{
    protected override void Invoke(object parameter)
    {
        var globalTweenHandler = App.GlobalTweenHandler;
        globalTweenHandler.ClearTweensOf(TargetElement);

        var anchor = Anchor.Position(TargetElement);
        var scaleTransform = new ScaleTransform(1, 1, anchor.X, anchor.Y);
        globalTweenHandler.Add(scaleTransform.Tween(x => x.ScaleX).To(0.2).In(Duration).Ease(Easing.ExpoEaseInOut));
        globalTweenHandler.Add(scaleTransform.Tween(x => x.ScaleY).To(0.7).In(Duration).Ease(Easing.ExpoEaseInOut));
        globalTweenHandler.Add(TargetElement.Tween(x => x.Opacity).To(0).In(Duration).Ease(Easing.ExpoEaseInOut));

        TargetElement.RenderTransform = scaleTransform;
    }
}