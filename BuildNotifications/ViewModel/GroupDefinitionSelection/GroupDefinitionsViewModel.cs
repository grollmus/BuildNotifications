using System;
using System.Collections.ObjectModel;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionsViewModel : BaseViewModel
    {
        public GroupDefinitionsViewModel()
        {
            Definitions = new ObservableCollection<GroupDefinitionViewModel>
            {
                new GroupDefinitionViewModel(GroupDefinition.Branch),
                new GroupDefinitionViewModel(GroupDefinition.BuildDefinition),
                new GroupDefinitionViewModel(GroupDefinition.Source),
                new GroupDefinitionViewModel(GroupDefinition.None)
            };

            _selectedDefinition = Definitions.First(x => x.GroupDefinition == GroupDefinition.None);
            SelectedDefinition = _selectedDefinition;
        }

        public ObservableCollection<GroupDefinitionViewModel> Definitions { get; }

        public string GroupByText
        {
            get => Definitions.FirstOrDefault()?.GroupByText ?? "";
            set
            {
                foreach (var definition in Definitions)
                {
                    definition.GroupByText = value;
                }
            }
        }

        public GroupDefinitionViewModel? SelectedDefinition
        {
            get => _selectedDefinition;
            set
            {
                if (value == null)
                    return;

                var oldValue = _selectedDefinition;
                oldValue.IsSelected = false;
                oldValue.SelectedSortingDefinitionChanged -= OnSelectedSortingDefinitionChanged;

                _selectedDefinition = value;
                _selectedDefinition.IsSelected = true;
                _selectedDefinition.SelectedSortingDefinitionChanged += OnSelectedSortingDefinitionChanged;

                OnPropertyChanged();
                SelectedDefinitionChanged?.Invoke(this, new GroupDefinitionsSelectionChangedEventArgs(oldValue, value));
            }
        }

        public SortingDefinition? SelectedSortingDefinition
        {
            get => SelectedDefinition?.SortingDefinitionsViewModel.SelectedSortingDefinition;
            set
            {
                if (SelectedDefinition != null)
                    SelectedDefinition.SortingDefinitionsViewModel.SelectedSortingDefinition = value;
            }
        }

        public event EventHandler<GroupDefinitionsSelectionChangedEventArgs>? SelectedDefinitionChanged;
        public event EventHandler<SortingDefinitionsSelectionChangedEventArgs>? SelectedSortingDefinitionChanged;

        private void OnSelectedSortingDefinitionChanged(object? sender, SortingDefinitionsSelectionChangedEventArgs e)
        {
            SelectedSortingDefinitionChanged?.Invoke(sender, e);
        }

        private GroupDefinitionViewModel _selectedDefinition;
    }
}