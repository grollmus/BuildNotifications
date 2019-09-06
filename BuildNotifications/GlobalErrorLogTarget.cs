using System;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Notification;
using NLog;
using NLog.Targets;

namespace BuildNotifications
{
    [Target("GlobalErrorLog")]
    public sealed class GlobalErrorLogTarget : TargetWithLayout
    {
        public static event EventHandler<ErrorNotificationEventArgs> ErrorOccured;

        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = Layout.Render(logEvent);
            var notification = new ErrorNotification(logMessage);
            ErrorOccured?.Invoke(this, new ErrorNotificationEventArgs(notification));
        }
    }
}