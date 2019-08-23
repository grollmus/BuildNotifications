using System;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.Core.Pipeline
{
    internal class PipelineNotifier : IPipelineNotifier
    {
        public void Notify(IBuildTree tree)
        {
            Updated?.Invoke(this, new PipelineUpdateEventArgs(tree));
        }

        public void NotifyError(Exception exception, string messageTextId, params string[] messageParameter)
        {
            ErrorOccured?.Invoke(this, new PipelineErrorEventArgs(exception, messageTextId, messageParameter));
        }

        /// <inheritdoc />
        public event EventHandler<PipelineUpdateEventArgs> Updated;

        public event EventHandler<PipelineErrorEventArgs> ErrorOccured;
    }
}