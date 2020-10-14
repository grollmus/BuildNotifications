using System;
using System.Linq;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Notification;
using NLog;
using NLog.Targets;

namespace BuildNotifications
{
    [Target("GlobalErrorLog")]
    public sealed class GlobalErrorLogTarget : TargetWithLayout
    {
        public static event EventHandler<ErrorNotificationEventArgs>? ErrorOccurred;

        protected override void Write(LogEventInfo logEvent)
        {
            if (_isMuted)
                return;

            var logMessage = Layout.Render(logEvent);
            var source = logEvent.LoggerName?.Split('.').Last() ?? "";
            var notification = new ErrorNotification(logMessage) {Source = source};
            ErrorOccurred?.Invoke(this, new ErrorNotificationEventArgs(notification));
        }

        public static IDisposable Mute()
        {
            return new MuteLog();
        }

        private static bool _isMuted;

        private class MuteLog : IDisposable
        {
            public MuteLog()
            {
                _isMuted = true;
            }

            public void Dispose()
            {
                _isMuted = false;
            }
        }
    }
}