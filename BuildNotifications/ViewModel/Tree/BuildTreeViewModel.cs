using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree
{
    public class BuildTreeViewModel : BuildTreeNodeViewModel
    {
        private IBuildTree _tree;

        public BuildTreeViewModel(IBuildTree tree) : base(tree)
        {
            _tree = tree;
        }
    }
}
