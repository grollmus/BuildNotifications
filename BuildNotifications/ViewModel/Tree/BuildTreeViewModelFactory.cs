using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree
{
    public class BuildTreeViewModelFactory
    {
        public BuildTreeViewModel Produce(IBuildTree tree)
        {
            var groupsAsList = tree.GroupDefinition.ToList();
            var buildTree = new BuildTreeViewModel(tree);
            var children = CreateChildren(tree.Children, groupsAsList);
            foreach (var childVm in children)
            {
                buildTree.Children.Add(childVm);
            }

            var treeDepth = SetCurrentDepths(buildTree);
            SetMaxDepths(buildTree, treeDepth);

            return buildTree;
        }

        private static void SetMaxDepths(BuildTreeNodeViewModel node, in int maxDepth)
        {
            node.MaxTreeDepth = maxDepth;
            foreach (var child in node.Children)
            {
                SetMaxDepths(child, maxDepth);
            }
        }

        private static int SetCurrentDepths(BuildTreeNodeViewModel node, int currentDepth = 0)
        {
            node.CurrentTreeLevelDepth = currentDepth;

            var maxDepth = currentDepth;
            foreach (var child in node.Children)
            {
                var deepestChildDepth = SetCurrentDepths(child, currentDepth + 1);
                maxDepth = deepestChildDepth;
            }

            return maxDepth;
        }

        private IEnumerable<BuildTreeNodeViewModel> CreateChildren(IEnumerable<IBuildTreeNode> children, IReadOnlyList<GroupDefinition> groups, int groupIndex = 0)
        {
            if (children == null)
                yield break;

            foreach (var node in children)
            {
                var groupDefinition = groupIndex >= groups.Count ? GroupDefinition.None : groups[groupIndex];
                var nodeVm = AsViewModel(node, groupDefinition);
                var childrenVms = CreateChildren(node.Children, groups, groupIndex + 1);
                foreach (var childVm in childrenVms)
                {
                    nodeVm.Children.Add(childVm);
                }

                yield return nodeVm;
            }
        }

        private static BuildTreeNodeViewModel AsViewModel(IBuildTreeNode node, GroupDefinition groupDefinition)
        {
            switch (groupDefinition)
            {
                case GroupDefinition.Branch:
                    return new BranchGroupNodeViewModel(node as IBranchGroupNode);
                case GroupDefinition.BuildDefinition:
                    return new BuildDefinitionGroupNodeViewModel(node);
                case GroupDefinition.Source:
                    return new SourceGroupNodeViewModel(node);
                case GroupDefinition.Status:
                    return new StatusGroupNodeViewModel(node);
                case GroupDefinition.None:
                default:
                    return new BuildNodeViewModel(node as IBuildNode);
            }
        }
    }
}