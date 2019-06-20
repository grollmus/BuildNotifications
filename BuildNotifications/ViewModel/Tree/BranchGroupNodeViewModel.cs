using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree
{
    public class BranchGroupNodeViewModel : BuildTreeNodeViewModel
    {
        private readonly IBranchGroupNode _node;

        public string BranchName => _node.BranchName;

        public BranchGroupNodeViewModel(IBranchGroupNode node)
        {
            _node = node;
        }
    }
}