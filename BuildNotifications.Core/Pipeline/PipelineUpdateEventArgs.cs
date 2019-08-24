using System;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.Core.Pipeline
{
    public class PipelineUpdateEventArgs : EventArgs
    {
        public PipelineUpdateEventArgs(IBuildTree tree, IBuildTreeBuildsDelta delta)
        {
            Tree = tree;
            Delta = delta;
        }

        public IBuildTree Tree { get; }
        public IBuildTreeBuildsDelta Delta { get; }
    }
}