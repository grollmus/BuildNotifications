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
    }
}