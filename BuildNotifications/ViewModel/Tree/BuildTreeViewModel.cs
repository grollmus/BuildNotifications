using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.ViewModel.Tree
{
    public class BuildTreeViewModel : BuildTreeNodeViewModel
    {
        private IBuildTree _tree;
        private IBuildTreeSortingDefinition _sortingDefinition;

        public BuildTreeViewModel(IBuildTree tree) : base(tree)
        {
            _tree = tree;
        }

        public IBuildTreeSortingDefinition SortingDefinition
        {
            get => _sortingDefinition;
            set
            {
                _sortingDefinition = value;
                SetSortings(value.ToList());
                OnPropertyChanged();
            }
        }
    }
}
