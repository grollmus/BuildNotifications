using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class SortingDefinitionsViewModel : BaseViewModel
    {
        public GroupDefinition ForGroupDefinition { get; }
        private SortingDefinitionViewModel _selectedViewModel;
        public RemoveTrackingObservableCollection<SortingDefinitionViewModel> Sortings { get; set; }

        public SortingDefinitionViewModel SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                _selectedViewModel = value;
                OnPropertyChanged();
            }
        }

        public SortingDefinition SelectedSortingDefinition
        {
            get => SelectedViewModel.SortingDefinition;
            set => SelectedViewModel = Sortings.First(x => Equals(x.SortingDefinition, value));
        }

        public SortingDefinitionsViewModel(GroupDefinition forGroupDefinition)
        {
            ForGroupDefinition = forGroupDefinition;
            Sortings = new RemoveTrackingObservableCollection<SortingDefinitionViewModel>
            {
                new SortingDefinitionViewModel(forGroupDefinition, SortingDefinition.AlphabeticalDescending),
                new SortingDefinitionViewModel(forGroupDefinition, SortingDefinition.AlphabeticalAscending),
                new SortingDefinitionViewModel(forGroupDefinition, SortingDefinition.StatusAscending),
                new SortingDefinitionViewModel(forGroupDefinition, SortingDefinition.StatusDescending),
            };
            SelectedViewModel = Sortings.First();
        }
    }
}