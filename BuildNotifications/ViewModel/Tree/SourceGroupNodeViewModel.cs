using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree
{
    public class SourceGroupNodeViewModel : BuildTreeNodeViewModel
    {
        private readonly ISourceGroupNode _node;

        public SourceGroupNodeViewModel(ISourceGroupNode node) : base(node)
        {
            _node = node;
        }

        protected override string CalculateDisplayName() => _node.SourceName;
    }
}