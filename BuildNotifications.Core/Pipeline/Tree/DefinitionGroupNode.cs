using System;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree;

internal class DefinitionGroupNode : TreeNode, IDefinitionGroupNode
{
    public DefinitionGroupNode(IBuildDefinition definition)
    {
        Definition = definition;
    }

    public IBuildDefinition Definition { get; }

    public override bool Equals(IBuildTreeNode other) => base.Equals(other) && Definition.Id.Equals((other as DefinitionGroupNode)?.Definition.Id, StringComparison.InvariantCulture);
}