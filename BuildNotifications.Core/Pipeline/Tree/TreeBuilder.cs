using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
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
            IBuildTreeNode? node = null;

            foreach (var group in GroupDefinition.Reverse())
            {
                var parent = ConstructNode(group, build);

                if (node != null)
                {
                    parent.AddChild(node);
                }

                node = parent;
            }

            return node;
        }

        private static IBuildTreeNode ConstructNode(GroupDefinition group, IBuild build)
        {
            switch (group)
            {
                case Tree.GroupDefinition.Branch:
                    return new BranchGroupNode(build.BranchName);
                case Tree.GroupDefinition.BuildDefinition:
                    return new DefinitionGroupNode(build.Definition);
                //case Tree.GroupDefinition.Source:
                //    throw new NotImplementedException();

                default:
                    return new BuildNode(build);
            }
        }

        private void Merge(IBuildTreeNode tree, IBuildTreeNode path)
        {
            var target = tree.Children.FirstOrDefault(c => c == path);
            if (target == null)
            {
                tree.AddChild(path);
            }
            else
            {
                foreach (var child in path.Children)
                {
                    Merge(target, child);
                }
            }
        }

        /// <inheritdoc />
        public IBuildTree Build(IEnumerable<IBuild> builds, IEnumerable<IBranch> branches, IEnumerable<IBuildDefinition> definitions)
        {
            var tree = new BuildTree(GroupDefinition);

            foreach (var build in builds)
            {
                var path = BuildPath(build);
                if (path == null)
                {
                    continue;
                }

                Merge(tree, path);
            }

            return tree;
        }

        private readonly IConfiguration _config;
    }
}