using System;
using System.Collections.Generic;

namespace BuildNotifications.Core.Pipeline.Tree
{
    public interface IBuildTreeNode : IEquatable<IBuildTreeNode>
    {
        IEnumerable<IBuildTreeNode> Children { get; }
        void AddChild(IBuildTreeNode node);
        void RemoveChild(IBuildTreeNode node);
    }
}