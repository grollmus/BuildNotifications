using System.Collections.Generic;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal interface IBuildTreeNode
    {
        IEnumerable<IBuildTreeNode> Children { get; }
    }
}