using System;
using System.Windows.Media;
using TweenSharp;

namespace BuildNotifications
{
    public partial class App
    {
        public static TweenHandler GlobalTweenHandler;
        private static TimeSpan _lastUpdate;

        public App()
        {
            GlobalTweenHandler = new TweenHandler();
            CompositionTarget.Rendering += CompositionTargetOnRendering;
            _lastUpdate = TimeSpan.Zero;
        }

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            var renderEventArgs = e as RenderingEventArgs;
            if (renderEventArgs == null)
                return;

            if (_lastUpdate == renderEventArgs.RenderingTime)
                return;

            var delta = renderEventArgs.RenderingTime - _lastUpdate;
            _lastUpdate = renderEventArgs.RenderingTime;
            
            GlobalTweenHandler.Update(delta.Milliseconds);
        }
    }
}