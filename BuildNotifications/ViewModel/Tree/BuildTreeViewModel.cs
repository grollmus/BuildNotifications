using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree
{
    internal class BuildTreeViewModel : BuildTreeNodeViewModel
    {
        private IBuildTree _tree;

        public BuildTreeViewModel(IBuildTree tree)
        {
            _tree = tree;
        }
    }

    internal abstract class BuildTreeNodeViewModel
    {
        public ObservableCollection<BuildTreeNodeViewModel> Children { get; }
    }

    internal class BranchGroupTreeNodeViewModel : BuildTreeNodeViewModel
    {

    }

    internal class BuildDefinitionGroupTreeNodeViewModel : BuildTreeNodeViewModel
    {

    }

    internal class SourceGroupTreeNodeViewModel : BuildTreeNodeViewModel
    {

    }

    internal class StatusGroupTreeNodeViewModel : BuildTreeNodeViewModel
    {

    }

    internal class BuildTreeViewModelFactory
    {
        BuildTreeViewModel Produce(IBuildTree tree)
        {
            var groupsAsList = tree.GroupDefinition.ToList();
            var buildTree = new BuildTreeViewModel(tree);
            var children = CreateChildren(tree.Children, groupsAsList);
            foreach (var childVm in children)
            {
                buildTree.Children.Add(childVm);
            }

            return buildTree;
        }

        private IEnumerable<BuildTreeNodeViewModel> CreateChildren(IEnumerable<IBuildTreeNode> children, IReadOnlyList<GroupDefinition> groups, int groupIndex = 0)
        {
            foreach (var node in children)
            {
                var nodeVm = AsViewModel(node, groups[groupIndex]);
                var childrenVms = CreateChildren(node.Children, groups, ++groupIndex);
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
                    return new BranchGroupTreeNodeViewModel();
                case GroupDefinition.BuildDefinition:
                    return new BuildDefinitionGroupTreeNodeViewModel();
                case GroupDefinition.Source:
                    return new SourceGroupTreeNodeViewModel();
                case GroupDefinition.Status:
                    return new StatusGroupTreeNodeViewModel();
                default:
                    throw new ArgumentOutOfRangeException(nameof(groupDefinition), groupDefinition, null);
            }
        }
    }
}
