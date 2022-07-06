using System;
using System.Collections.Generic;

namespace BuildNotifications.Core.Pipeline.Tree;

public interface IBuildTreeNode : IEquatable<IBuildTreeNode>
{
    IEnumerable<IBuildTreeNode> Children { get; }

    /// <summary>
    /// Indicates at what depth in the BuildTree this node is
    /// </summary>
    int Depth { get; set; }

    void AddChild(IBuildTreeNode node);

    IEnumerable<IBuildTreeNode> AllChildren();
    void RemoveChild(IBuildTreeNode node);
    void UpdateWithValuesFrom(IBuildTreeNode nodeToInsert);
}