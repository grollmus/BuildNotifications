using System.Collections.Generic;

namespace BuildNotifications.Core.Pipeline.Tree
{
    public interface IBuildTreeNode
    {
        IEnumerable<IBuildTreeNode> Children { get; }
    }
}