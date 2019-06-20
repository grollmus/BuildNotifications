using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree.Dummy
{
    internal class BuildTreeDummy : BuildTreeNodeDummy, IBuildTree
    {
        public BuildTreeDummy(IBuildTreeGroupDefinition groupDefinition)
        {
            GroupDefinition = groupDefinition;
        }

        public IBuildTreeGroupDefinition GroupDefinition { get; }
    }
}