using System;
using System.Windows.Media;
using NLog.Config;
using TweenSharp;

namespace BuildNotifications
{
    public partial class App
    {
        public App()
        {
            GlobalTweenHandler = new TweenHandler();
            CompositionTarget.Rendering += CompositionTargetOnRendering;
            _lastUpdate = TimeSpan.Zero;

            // setup global event logger
            ConfigurationItemFactory
                .Default
                .Targets
                .RegisterDefinition("GlobalErrorLog", typeof(GlobalErrorLogTarget));
        }

        private void CompositionTargetOnRendering(object? sender, EventArgs e)
        {
            var renderEventArgs = e as RenderingEventArgs;
            if (renderEventArgs == null)
                return;

            if (_lastUpdate == renderEventArgs.RenderingTime)
                return;

            var delta = renderEventArgs.RenderingTime - _lastUpdate;
            _lastUpdate = renderEventArgs.RenderingTime;

            // for lag spikes, don't skip frames faster than 20fps (50ms per frame)
            const int maxTimePerFrame = 50;
            var tooMuchTime = delta.Milliseconds - maxTimePerFrame;
            if (tooMuchTime > 0)
            {
                // ignore super big spikes
                if (tooMuchTime > 200)
                    tooMuchTime = 200;

                _lastUpdate -= TimeSpan.FromMilliseconds(tooMuchTime);
                delta = TimeSpan.FromMilliseconds(maxTimePerFrame);
            }

            GlobalTweenHandler.Update(delta.Milliseconds);
        }

        private static TimeSpan _lastUpdate;
        public static TweenHandler GlobalTweenHandler;
    }
}