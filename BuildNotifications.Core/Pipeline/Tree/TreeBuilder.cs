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

        private void Merge(IBuildTreeNode tree, IBuildTreeNode nodeToInsert)
        {
            var insertTarget = tree;

            var subTree = insertTarget.Children.FirstOrDefault(node => node.Equals(nodeToInsert));
            if (subTree != null)
                Merge(subTree, nodeToInsert.Children.First());
            else
                tree.AddChild(nodeToInsert);
        }

        /// <inheritdoc />
        public IBuildTree Build(IEnumerable<IBuild> builds, IEnumerable<IBranch> branches, IEnumerable<IBuildDefinition> definitions)
        {
            var tree = new BuildTree(GroupDefinition);

            foreach (var build in builds)
            {
                var path = BuildPath(build);
                if (path == null)
                    continue;

                Merge(tree, path);
            }

            return tree;
        }

        private readonly IConfiguration _config;
    }
}