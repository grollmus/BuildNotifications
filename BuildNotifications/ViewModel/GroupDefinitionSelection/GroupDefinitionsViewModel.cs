using System;
using System.Collections.ObjectModel;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionsViewModel : BaseViewModel
    {
        private GroupDefinitionViewModel _selectedDefinition;
        public ObservableCollection<GroupDefinitionViewModel> Definitions { get; set; }

        public event EventHandler<GroupDefinitionsSelectionChangedEventArgs> SelectedDefinitionChanged;
        public event EventHandler<SortingDefinitionsSelectionChangedEventArgs> SelectedSortingDefinitionChanged;

        public GroupDefinitionViewModel SelectedDefinition
        {
            get => _selectedDefinition;
            set
            {
                var oldValue = _selectedDefinition;
                if (oldValue != null)
                {
                    oldValue.IsSelected = false;
                    oldValue.SelectedSortingDefinitionChanged -= OnSelectedSortingDefinitionChanged;
                }

                _selectedDefinition = value;
                if (_selectedDefinition != null)
                {
                    _selectedDefinition.IsSelected = true;
                    _selectedDefinition.SelectedSortingDefinitionChanged += OnSelectedSortingDefinitionChanged;
                }

                OnPropertyChanged();
                SelectedDefinitionChanged?.Invoke(this, new GroupDefinitionsSelectionChangedEventArgs(oldValue, value));
            }
        }

        private void OnSelectedSortingDefinitionChanged(object sender, SortingDefinitionsSelectionChangedEventArgs e)
        {
            SelectedSortingDefinitionChanged?.Invoke(sender, e);
        }

        public SortingDefinition SelectedSortingDefinition
        {
            get => SelectedDefinition.SortingDefinitionsViewModel.SelectedSortingDefinition;
            set => SelectedDefinition.SortingDefinitionsViewModel.SelectedSortingDefinition = value;
        }

        public string GroupByText
        {
            get => Definitions.FirstOrDefault()?.GroupByText;
            set
            {
                foreach (var definition in Definitions)
                {
                    definition.GroupByText = value;
                }
            }
        }

        public GroupDefinitionsViewModel()
        {
            Definitions = new ObservableCollection<GroupDefinitionViewModel>
            {
                new GroupDefinitionViewModel(GroupDefinition.Branch),
                new GroupDefinitionViewModel(GroupDefinition.BuildDefinition),
                new GroupDefinitionViewModel(GroupDefinition.Source),
                new GroupDefinitionViewModel(GroupDefinition.Status),
                new GroupDefinitionViewModel(GroupDefinition.None),
            };
            SelectedDefinition = Definitions.First(x => x.GroupDefinition == GroupDefinition.None);
        }
    }
}
