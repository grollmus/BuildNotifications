using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;

namespace BuildNotifications.Core.Pipeline
{
    public class ErrorNotificationEventArgs : EventArgs
    {
        public ErrorNotificationEventArgs(params INotification[] errorNotifications)
        {
            ErrorNotifications = errorNotifications;
        }

        public IEnumerable<INotification> ErrorNotifications { get; }
    }
}