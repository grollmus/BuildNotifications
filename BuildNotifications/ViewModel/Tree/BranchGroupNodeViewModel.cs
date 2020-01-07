using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree
{
    public class BranchGroupNodeViewModel : BuildTreeNodeViewModel
    {
        public BranchGroupNodeViewModel(IBranchGroupNode node)
            : base(node)
        {
            _node = node;
        }

        public string BranchName => _node.BranchName;

        protected override string CalculateDisplayName()
        {
            return BranchName;
        }

        private readonly IBranchGroupNode _node;
    }
}