using System.Windows.Controls;
using System.Windows.Interactivity;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.Resources.BuildTree.Build
{
    internal class BuildGrow : TriggerAction<Grid>
    {
        protected override void Invoke(object parameter)
        {
            var globalTweenHandler = App.GlobalTweenHandler;
            globalTweenHandler.ClearTweensOf(AssociatedObject);

            var targetWidth = (double) AssociatedObject.FindResource("BlockTriple");
            globalTweenHandler.Add(AssociatedObject.Tween(x => x.Width).To(targetWidth).In(0.25).Ease(Easing.QuadraticEaseInOut));
        }
    }
}