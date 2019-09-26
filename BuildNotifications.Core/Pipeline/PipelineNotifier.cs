using System;
using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.Core.Pipeline
{
    internal class PipelineNotifier : IPipelineNotifier
    {
        public void Notify(IBuildTree tree, IEnumerable<INotification> delta)
        {
            Updated?.Invoke(this, new PipelineUpdateEventArgs(tree, delta));
        }

        public event EventHandler<PipelineUpdateEventArgs>? Updated;
    }
}