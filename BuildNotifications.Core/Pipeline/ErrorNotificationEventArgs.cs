using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;

namespace BuildNotifications.Core.Pipeline
{
    public class ErrorNotificationEventArgs : EventArgs
    {
        public IEnumerable<INotification> ErrorNotifications { get; }

        public ErrorNotificationEventArgs(params INotification[] errorNotifications)
        {
            ErrorNotifications = errorNotifications;
        }
    }
}
