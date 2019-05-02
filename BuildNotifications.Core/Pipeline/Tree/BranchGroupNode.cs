using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class BranchGroupNode : TreeNode, IBranchGroupNode
    {
        public BranchGroupNode(IBranch branch)
        {
            Branch = branch;
        }

        /// <inheritdoc />
        public IBranch Branch { get; }
    }
}