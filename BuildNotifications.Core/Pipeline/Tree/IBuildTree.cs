namespace BuildNotifications.Core.Pipeline.Tree
{
    internal interface IBuildTree : IBuildTreeNode
    {
        IBuildTreeGroupDefinition GroupDefinition { get; }
    }
}