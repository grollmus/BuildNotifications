using System;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class SortingDefinitionsViewModel : BaseViewModel
    {
        public SortingDefinitionsViewModel(GroupDefinition forGroupDefinition)
        {
            ForGroupDefinition = forGroupDefinition;
            Sortings = new RemoveTrackingObservableCollection<SortingDefinitionViewModel>
            {
                new SortingDefinitionViewModel(forGroupDefinition, SortingDefinition.AlphabeticalDescending),
                new SortingDefinitionViewModel(forGroupDefinition, SortingDefinition.AlphabeticalAscending),
                new SortingDefinitionViewModel(forGroupDefinition, SortingDefinition.StatusDescending),
                new SortingDefinitionViewModel(forGroupDefinition, SortingDefinition.DateDescending)
            };
            _selectedViewModel = Sortings.First();
            SelectedViewModel = _selectedViewModel;
        }

        public GroupDefinition ForGroupDefinition { get; }

        public SortingDefinition? SelectedSortingDefinition
        {
            get => SelectedViewModel?.SortingDefinition;
            set => SelectedViewModel = Sortings.First(x => Equals(x.SortingDefinition, value));
        }

        public SortingDefinitionViewModel? SelectedViewModel
        {
            get => _selectedViewModel;
            set
            {
                if (value == null)
                    return;

                var oldValue = _selectedViewModel;
                _selectedViewModel = value;
                OnPropertyChanged();
                SelectedSortingDefinitionChanged?.Invoke(this, new SortingDefinitionsSelectionChangedEventArgs(oldValue, value));
            }
        }

        public RemoveTrackingObservableCollection<SortingDefinitionViewModel> Sortings { get; }

        public event EventHandler<SortingDefinitionsSelectionChangedEventArgs>? SelectedSortingDefinitionChanged;
        private SortingDefinitionViewModel _selectedViewModel;
    }
}