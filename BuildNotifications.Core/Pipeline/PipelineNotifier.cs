using System;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.Core.Pipeline
{
    internal class PipelineNotifier : IPipelineNotifier
    {
        public void Notify(IBuildTree tree, IBuildTreeBuildsDelta delta)
        {
            Updated?.Invoke(this, new PipelineUpdateEventArgs(tree, delta));
        }

        public void NotifyError(Exception exception, string messageTextId, params string[] messageParameter)
        {
            ErrorOccured?.Invoke(this, new PipelineErrorEventArgs(exception, messageTextId, messageParameter));
        }

        public event EventHandler<PipelineUpdateEventArgs> Updated;

        public event EventHandler<PipelineErrorEventArgs> ErrorOccured;
    }
}