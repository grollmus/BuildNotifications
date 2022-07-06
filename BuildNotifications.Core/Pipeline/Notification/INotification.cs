using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification;

public interface INotification
{
    IList<IBuildNode> BuildNodes { get; }

    string ContentTextId { get; }

    string DisplayContent { get; }

    string DisplayTitle { get; }

    Guid Guid { get; }

    /// <summary>
    /// Describes the root of the notification. E.g. a branch failed notification, the IssueSource would be
    /// the name of the
    /// branch.
    /// </summary>
    string IssueSource { get; }

    /// <summary>
    /// Source of the notification, e.g. the class for an error or the project name of builds.
    /// </summary>
    string Source { get; }

    BuildStatus Status { get; }

    string TitleTextId { get; }

    NotificationType Type { get; }
}