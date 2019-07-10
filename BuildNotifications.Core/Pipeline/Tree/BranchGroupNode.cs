namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class BranchGroupNode : TreeNode, IBranchGroupNode
    {
        public BranchGroupNode(string branchName)
        {
            BranchName = branchName;
        }

        /// <inheritdoc />
        public string BranchName { get; }

        public override bool Equals(IBuildTreeNode other)
        {
            return BranchName.Equals((other as BranchGroupNode)?.BranchName);
        }
    }
}