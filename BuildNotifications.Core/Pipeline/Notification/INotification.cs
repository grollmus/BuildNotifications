using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification
{
    public interface INotification
    {
        string DisplayContent { get; }

        string DisplayTitle { get; }

        string ContentTextId { get; }

        string TitleTextId { get; }
        
        /// <summary>
        /// Describes the root of the notification. E.g. a branch failed notification, the IssueSource would be the name of the branch.
        /// </summary>
        string IssueSource { get; }

        NotificationType Type { get; }

        IList<IBuildNode> BuildNodes { get; }

        BuildStatus Status { get; }

        Guid Guid { get; }
    }
}
