using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class TreeBuilder : ITreeBuilder
    {
        public TreeBuilder(IConfiguration config)
        {
            _config = config;
        }

        private IBuildTreeGroupDefinition GroupDefinition => _config.GroupDefinition;

        private IBuildTreeNode? BuildPath(IBuild build)
        {
            var node = ConstructNode(Arrangement.GroupDefinition.None, build);

            foreach (var group in GroupDefinition.Reverse())
            {
                var parent = ConstructNode(group, build);
                parent.AddChild(node);
                node = parent;
            }

            return node;
        }

        private static IBuildTreeNode ConstructNode(GroupDefinition group, IBuild build)
        {
            switch (group)
            {
                case Arrangement.GroupDefinition.Branch:
                    return new BranchGroupNode(build.BranchName);
                case Arrangement.GroupDefinition.BuildDefinition:
                    return new DefinitionGroupNode(build.Definition);
                case Arrangement.GroupDefinition.Source:
                    return new SourceGroupNode(build.ProjectName);
                default:
                    return new BuildNode(build);
            }
        }

        private void Merge(IBuildTreeNode tree, IBuildTreeNode nodeToInsert, List<IBuildTreeNode> taggedNodes)
        {
            var insertTarget = tree;

            var subTree = insertTarget.Children.FirstOrDefault(node => node.Equals(nodeToInsert));
            if (subTree != null)
            {
                if (nodeToInsert.Children.Any())
                {
                    Merge(subTree, nodeToInsert.Children.First(), taggedNodes);
                }

                taggedNodes.Remove(subTree);
            }
            else
            {
                tree.AddChild(nodeToInsert);
                taggedNodes.Remove(tree);
            }
        }

        private void RemoveTaggedNodes(IBuildTreeNode tree, List<IBuildTreeNode> taggedNodes)
        {
            foreach (var node in tree.Children.ToList())
            {
                if (taggedNodes.Contains(node))
                    tree.RemoveChild(node);
                else
                    RemoveTaggedNodes(node, taggedNodes);
            }
        }

        private void TagAllNodesForDeletion(IBuildTreeNode tree, List<IBuildTreeNode> taggedNodes)
        {
            foreach (var node in tree.Children)
            {
                taggedNodes.Add(node);
                TagAllNodesForDeletion(node, taggedNodes);
            }
        }

        /// <inheritdoc />
        public IBuildTree Build(IEnumerable<IBuild> builds, IEnumerable<IBranch> branches, IEnumerable<IBuildDefinition> definitions, IBuildTree? oldTree = null)
        {
            var tree = oldTree ?? new BuildTree(GroupDefinition);

            var taggedNodes = new List<IBuildTreeNode>();
            TagAllNodesForDeletion(tree, taggedNodes);

            foreach (var build in builds)
            {
                var path = BuildPath(build);
                if (path == null)
                    continue;

                Merge(tree, path, taggedNodes);
            }

            RemoveTaggedNodes(tree, taggedNodes);

            return tree;
        }

        private readonly IConfiguration _config;
    }
}