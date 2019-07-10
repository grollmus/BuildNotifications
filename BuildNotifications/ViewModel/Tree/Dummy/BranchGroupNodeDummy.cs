using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree.Dummy
{
    internal class BranchGroupNodeDummy : BuildTreeNodeDummy, IBranchGroupNode
    {
        public BranchGroupNodeDummy(string branchName)
        {
            BranchName = branchName;
        }

        public string BranchName { get; }
    }
}