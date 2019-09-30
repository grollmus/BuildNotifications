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
        public static event EventHandler<ErrorNotificationEventArgs>? ErrorOccured;

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = Layout.Render(logEvent);
            var source = logEvent?.LoggerName?.Split('.').Last() ?? "";
            var notification = new ErrorNotification(logMessage) {Source = source};
            ErrorOccured?.Invoke(this, new ErrorNotificationEventArgs(notification));
        }
    }
}