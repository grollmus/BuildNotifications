using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.Core.Pipeline.Tree
{
    public interface IBuildTree : IBuildTreeNode
    {
        IBuildTreeGroupDefinition GroupDefinition { get; }
    }
}