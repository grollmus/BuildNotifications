using System;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class SortingDefinitionsViewModel : BaseViewModel
    {
        private SortingDefinitionViewModel _selectedViewModel;

        public GroupDefinition ForGroupDefinition { get; }

        public RemoveTrackingObservableCollection<SortingDefinitionViewModel> Sortings { get; set; }

        public event EventHandler<SortingDefinitionsSelectionChangedEventArgs> SelectedSortingDefinitionChanged;

        public SortingDefinitionViewModel SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                var oldValue = _selectedViewModel;
                _selectedViewModel = value;
                OnPropertyChanged();
                SelectedSortingDefinitionChanged?.Invoke(this, new SortingDefinitionsSelectionChangedEventArgs(oldValue, value));
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