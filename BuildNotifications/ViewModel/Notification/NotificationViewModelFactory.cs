using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Notification;

namespace BuildNotifications.ViewModel.Notification
{
    internal class NotificationViewModelFactory
    {
        public IEnumerable<NotificationViewModel> Produce(IEnumerable<INotification> notifications)
        {
            return notifications.Select(x => new NotificationViewModel(x));
        }
    }
}