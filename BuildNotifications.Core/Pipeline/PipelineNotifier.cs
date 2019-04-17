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

        /// <inheritdoc />
        public event EventHandler<PipelineUpdateEventArgs> Updated;
    }
}