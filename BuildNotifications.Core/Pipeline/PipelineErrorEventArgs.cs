using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;

namespace BuildNotifications.Core.Pipeline
{
    public class PipelineErrorEventArgs : EventArgs
    {
        public IEnumerable<INotification> ErrorNotifications { get; }

        public PipelineErrorEventArgs(params INotification[] errorNotifications)
        {
            ErrorNotifications = errorNotifications;
        }
    }
}