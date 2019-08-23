namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class BranchGroupNode : TreeNode, IBranchGroupNode
    {
        public BranchGroupNode(string branchName)
        {
            BranchName = branchName;
        }

        /// <inheritdoc />
        public string BranchName { get; private set; }

        public override void UpdateWithValuesFrom(IBuildTreeNode nodeToInsert)
        {
            BranchName = (nodeToInsert as BranchGroupNode)?.BranchName ?? string.Empty;
        }

        public override bool Equals(IBuildTreeNode other)
        {
            return base.Equals(other) && BranchName.Equals((other as BranchGroupNode)?.BranchName);
        }
    }
}