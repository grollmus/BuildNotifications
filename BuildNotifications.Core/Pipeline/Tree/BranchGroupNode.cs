using System;

namespace BuildNotifications.Core.Pipeline.Tree;

internal class BranchGroupNode : TreeNode, IBranchGroupNode
{
    public BranchGroupNode(string branchName, bool isPullRequest)
    {
        BranchName = branchName;
        IsPullRequest = isPullRequest;
    }

    public string BranchName { get; private set; }
    public bool IsPullRequest { get; }

    public override void UpdateWithValuesFrom(IBuildTreeNode nodeToInsert)
    {
        BranchName = (nodeToInsert as BranchGroupNode)?.BranchName ?? string.Empty;
    }

    public override bool Equals(IBuildTreeNode other) => base.Equals(other) && BranchName.Equals((other as BranchGroupNode)?.BranchName, StringComparison.InvariantCulture);
}