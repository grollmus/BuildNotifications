using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.ViewModel.Tree
{
    public class BuildTreeViewModel : BuildTreeNodeViewModel
    {
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

        private IBuildTree _tree;
        private IBuildTreeSortingDefinition _sortingDefinition;
    }
}