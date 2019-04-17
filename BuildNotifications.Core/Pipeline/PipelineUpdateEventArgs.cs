using System;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.Core.Pipeline
{
    public class PipelineUpdateEventArgs : EventArgs
    {
        public PipelineUpdateEventArgs(IBuildTree tree)
        {
            Tree = tree;
        }

        public IBuildTree Tree { get; }
    }
}