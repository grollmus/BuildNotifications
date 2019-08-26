using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.ViewModel.Tree
{
    internal class BuildTreeViewModelFactory
    {
        public async Task<BuildTreeViewModel> ProduceAsync(IBuildTree tree, BuildTreeViewModel existingTree)
        {
            var buildTreeResult = await Task.Run(() =>
            {
                var groupsAsList = tree.GroupDefinition.ToList();
                var buildTree = new BuildTreeViewModel(tree);
                var children = CreateChildren(tree.Children, groupsAsList);
                foreach (var childVm in children)
                {
                    buildTree.Children.Add(childVm);
                }

                return buildTree;
            });

            if (existingTree != null)
                buildTreeResult = Merge(existingTree, buildTreeResult);
            
            var treeDepth = GetMaxDepth(buildTreeResult);
            SetMaxDepths(buildTreeResult, treeDepth);
            
            return buildTreeResult;
        }

        private static BuildTreeNodeViewModel AsViewModel(IBuildTreeNode node, GroupDefinition groupDefinition)
        {
            switch (groupDefinition)
            {
                case GroupDefinition.Branch:
                    return new BranchGroupNodeViewModel((IBranchGroupNode) node);
                case GroupDefinition.BuildDefinition:
                    return new DefinitionGroupNodeViewModel((IDefinitionGroupNode) node);
                case GroupDefinition.Source:
                    return new SourceGroupNodeViewModel((ISourceGroupNode) node);
                case GroupDefinition.Status:
                    return new StatusGroupNodeViewModel(node);
                default:
                    return new BuildNodeViewModel((IBuildNode) node) {IsLargeSize = false};
            }
        }

        private IEnumerable<BuildTreeNodeViewModel> CreateChildren(IEnumerable<IBuildTreeNode> children, IReadOnlyList<GroupDefinition> groups, int groupIndex = 0)
        {
            if (children == null)
                yield break;

            foreach (var node in children.ToList())
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

        private BuildTreeViewModel Merge(BuildTreeViewModel tree1, BuildTreeViewModel tree2)
        {
            var taggedNodes = new List<BuildTreeNodeViewModel>();
            TagAllNodesForDeletion(tree1, taggedNodes);

            foreach (var child in tree2.Children)
            {
                MergeInternal(tree1, child, taggedNodes);
            }

            RemoveTaggedNodes(tree1, taggedNodes);

            return tree1;
        }

        private void MergeInternal(BuildTreeNodeViewModel tree1, BuildTreeNodeViewModel tree2, List<BuildTreeNodeViewModel> taggedNodes)
        {
            var insertTarget = tree1;
            var nodeToInsert = tree2;

            var subTree = insertTarget.Children.FirstOrDefault(node => node.NodeSource.Equals(nodeToInsert.NodeSource));
            if (subTree != null)
            {
                foreach (var child in nodeToInsert.Children)
                {
                    MergeInternal(subTree, child, taggedNodes);
                }

                taggedNodes.Remove(subTree);
                subTree.BackendPropertiesChanged();
            }
            else
            {
                tree1.Children.Add(nodeToInsert);
                taggedNodes.Remove(tree1);
            }
        }

        private void RemoveTaggedNodes(BuildTreeNodeViewModel tree, List<BuildTreeNodeViewModel> taggedNodes)
        {
            foreach (var node in tree.Children.ToList())
            {
                if (taggedNodes.Contains(node))
                    tree.Children.Remove(node);
                else
                    RemoveTaggedNodes(node, taggedNodes);
            }
        }

        private static int GetMaxDepth(BuildTreeNodeViewModel node, int currentDepth = 0)
        {
            var maxDepth = currentDepth;
            foreach (var child in node.Children)
            {
                var deepestChildDepth = GetMaxDepth(child, currentDepth + 1);
                maxDepth = deepestChildDepth;
            }

            return maxDepth;
        }

        private static void SetMaxDepths(BuildTreeNodeViewModel node, in int maxDepth)
        {
            node.MaxTreeDepth = maxDepth;
            foreach (var child in node.Children)
            {
                SetMaxDepths(child, maxDepth);
            }
        }

        private void TagAllNodesForDeletion(BuildTreeNodeViewModel tree, List<BuildTreeNodeViewModel> taggedNodes)
        {
            foreach (var node in tree.Children)
            {
                taggedNodes.Add(node);
                TagAllNodesForDeletion(node, taggedNodes);
            }
        }
    }
}