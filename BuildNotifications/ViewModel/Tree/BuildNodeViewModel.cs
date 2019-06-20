using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.Tree
{
    internal class BuildNodeViewModel : BuildTreeNodeViewModel
    {
        public IBuildNode Node { get; }

        public BuildNodeViewModel(IBuildNode node)
        {
            Node = node;
        }
    }
}