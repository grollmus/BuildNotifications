using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.Core.Pipeline.Tree;

internal class BuildTree : TreeNode, IBuildTree
{
    public BuildTree(IBuildTreeGroupDefinition groupDefinition)
    {
        GroupDefinition = groupDefinition;
    }

    public IBuildTreeGroupDefinition GroupDefinition { get; set; }
}