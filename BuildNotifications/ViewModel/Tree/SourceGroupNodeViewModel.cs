using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree
{
    public class SourceGroupNodeViewModel : BuildTreeNodeViewModel
    {
        public SourceGroupNodeViewModel(ISourceGroupNode node) : base(node)
        {
            _node = node;
        }

        protected override string CalculateDisplayName()
        {
            return _node.SourceName;
        }

        private readonly ISourceGroupNode _node;
    }
}