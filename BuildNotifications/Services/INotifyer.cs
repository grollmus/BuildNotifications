using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;

namespace BuildNotifications.Services
{
    interface INotifier
    {
        void ShowNotifications(IEnumerable<INotification> notifications);
    }
}