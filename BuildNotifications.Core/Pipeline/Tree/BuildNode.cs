using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class BuildNode : TreeNode, IBuildNode
    {
        public BuildNode(IBuild build)
        {
            Build = build;
        }

        /// <inheritdoc />
        public IBuild Build { get; }

        public override bool Equals(IBuildTreeNode other)
        {
            return Build.Id.Equals((other as BuildNode)?.Build?.Id);
        }
    }
}