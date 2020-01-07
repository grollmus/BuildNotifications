using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.ViewModel.Tree
{
    public class BuildTreeViewModel : BuildTreeNodeViewModel
    {
        public BuildTreeViewModel(IBuildTreeNode tree)
            : base(tree)
        {
            _sortingDefinition = new BuildTreeSortingDefinition();
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

        public IEnumerable<BuildNodeViewModel> AllBuilds()
        {
            return AllBuilds(this);
        }

        protected override string CalculateDisplayName()
        {
            return "";
        }

        private IEnumerable<BuildNodeViewModel> AllBuilds(BuildTreeNodeViewModel ofNode)
        {
            return ofNode.Children.OfType<BuildNodeViewModel>().Concat(ofNode.Children.SelectMany(AllBuilds));
        }

        private IBuildTreeSortingDefinition _sortingDefinition;
    }
}