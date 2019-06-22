using System;
using System.Windows.Media;
using TweenSharp;

namespace BuildNotifications
{
    public partial class App
    {
        public static TweenHandler GlobalTweenHandler;
        private static DateTime _lastUpdate;

        public App()
        {
            GlobalTweenHandler = new TweenHandler();
            CompositionTarget.Rendering += CompositionTargetOnRendering;
            _lastUpdate = DateTime.Now;
        }

        private void CompositionTargetOnRendering(object sender, EventArgs e)
        {
            var now = DateTime.Now;
            var delta = now - _lastUpdate;
            _lastUpdate = now;

            // set a maximum of 16 milliseconds to prevent too noticeable UI freezes
            var inMs = Math.Min(16, delta.Milliseconds);
            GlobalTweenHandler.Update(delta.Milliseconds);
        }
    }
}