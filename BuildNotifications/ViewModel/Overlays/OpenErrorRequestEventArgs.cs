using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;

namespace BuildNotifications.ViewModel.Overlays;

public class OpenErrorRequestEventArgs : EventArgs
{
    public OpenErrorRequestEventArgs(IEnumerable<INotification> errorNotifications)
    {
        ErrorNotifications = errorNotifications;
    }

    public IEnumerable<INotification> ErrorNotifications { get; set; }
}