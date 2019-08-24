using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.Core.Pipeline.Notification
{
    public interface INotification
    {
        string DisplayTitle { get; }

        string DisplayContent { get; }

        string TitleTextId { get; }

        string ContentTextId { get; }

        NotificationType Type { get; }

        IList<IBuildNode> BuildNodes { get; }
    }
}
