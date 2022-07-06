using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.Core.Pipeline;

public class PipelineUpdateEventArgs : EventArgs
{
    public PipelineUpdateEventArgs(IBuildTree tree, IEnumerable<INotification> notifications)
    {
        Tree = tree;
        Notifications = notifications;
    }

    public IEnumerable<INotification> Notifications { get; }

    public IBuildTree Tree { get; }
}