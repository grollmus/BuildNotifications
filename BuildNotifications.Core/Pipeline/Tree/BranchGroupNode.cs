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
    }
}