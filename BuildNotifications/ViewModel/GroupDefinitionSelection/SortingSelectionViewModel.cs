using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class SortingSelectionViewModel : BaseViewModel
    {
        public GroupDefinition ForGroupDefinition { get; }
        private GroupDefinitionSortingViewModel _selectedSorting;
        public RemoveTrackingObservableCollection<GroupDefinitionSortingViewModel> Sortings { get; set; }

        public GroupDefinitionSortingViewModel SelectedSorting
        {
            get => _selectedSorting;
            set
            {
                _selectedSorting = value;
                OnPropertyChanged();
            }
        }

        public SortingSelectionViewModel(GroupDefinition forGroupDefinition)
        {
            ForGroupDefinition = forGroupDefinition;
            Sortings = new RemoveTrackingObservableCollection<GroupDefinitionSortingViewModel>
            {
                new GroupDefinitionSortingViewModel(forGroupDefinition, SortingDefinition.AlphabeticalDescending),
                new GroupDefinitionSortingViewModel(forGroupDefinition, SortingDefinition.AlphabeticalAscending),
                new GroupDefinitionSortingViewModel(forGroupDefinition, SortingDefinition.StatusAscending),
                new GroupDefinitionSortingViewModel(forGroupDefinition, SortingDefinition.StatusDescending),
            };
            SelectedSorting = Sortings.First();
        }
    }
}