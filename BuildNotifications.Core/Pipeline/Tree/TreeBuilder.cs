using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class TreeBuilder : ITreeBuilder
    {
        public TreeBuilder(IConfiguration config)
        {
            _config = config;
        }

        private IBuildTreeGroupDefinition GroupDefinition => _config.GroupDefinition;

        private IBuildTreeNode BuildPath(IBuild build)
        {
            var node = ConstructNode(Arrangement.GroupDefinition.None, build);
            var currentDepth = GroupDefinition.Count();
            node.Depth = currentDepth + 1;

            foreach (var group in GroupDefinition.Reverse())
            {
                var parent = ConstructNode(group, build);
                parent.AddChild(node);
                node = parent;
                node.Depth = currentDepth;
                currentDepth -= 1;
            }

            return node;
        }

        private IBuildTreeNode ConstructNode(GroupDefinition group, IBuild build)
        {
            switch (group)
            {
                case Arrangement.GroupDefinition.Branch:
                {
                    var enrichedBuild = build as EnrichedBuild;
                    var isPullRequest = enrichedBuild?.Branch?.IsPullRequest ?? false;
                    var displayName = enrichedBuild?.Branch?.DisplayName ?? build.BranchName;
                    return new BranchGroupNode(displayName, isPullRequest);
                }
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
                subTree.UpdateWithValuesFrom(nodeToInsert);

                if (nodeToInsert.Children.Any())
                    Merge(subTree, nodeToInsert.Children.First(), taggedNodes);

                taggedNodes.RemoveAll(x => ReferenceEquals(x, subTree));
            }
            else
            {
                tree.AddChild(nodeToInsert);
                taggedNodes.RemoveAll(x => ReferenceEquals(x, tree));
            }
        }

        private void RemoveTaggedNodes(IBuildTreeNode tree, List<IBuildTreeNode> taggedNodes)
        {
            foreach (var node in tree.Children.ToList())
            {
                if (taggedNodes.Any(x => ReferenceEquals(x, node)))
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

        public IBuildTree Build(IEnumerable<IBuild> builds, IBuildTree? oldTree = null, ISpecificSearch? search = null)
        {
            search ??= new EmptySearch();
            var tree = oldTree ?? new BuildTree(GroupDefinition);

            if (tree.GroupDefinition != GroupDefinition)
                tree.GroupDefinition = GroupDefinition;

            var taggedNodes = new List<IBuildTreeNode>();
            TagAllNodesForDeletion(tree, taggedNodes);

            var filteredBuilds = search.ApplySearch(builds);

            foreach (var build in filteredBuilds)
            {
                var path = BuildPath(build);
                Merge(tree, path, taggedNodes);
            }

            RemoveTaggedNodes(tree, taggedNodes);

            return tree;
        }

        private readonly IConfiguration _config;
    }
}