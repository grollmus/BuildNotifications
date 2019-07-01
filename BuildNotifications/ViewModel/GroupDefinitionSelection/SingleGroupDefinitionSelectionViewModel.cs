using System;
using System.Collections.ObjectModel;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class SingleGroupDefinitionSelectionViewModel : BaseViewModel
    {
        private GroupDefinitionViewModel _selectedDefinition;
        private string _groupByText;
        public ObservableCollection<GroupDefinitionViewModel> Definitions { get; set; }

        public event EventHandler<GroupDefinitionSelectionChangedEventArgs> SelectedDefinitionChanged;

        public GroupDefinitionViewModel SelectedDefinition
        {
            get => _selectedDefinition;
            set
            {
                var oldValue = _selectedDefinition;
                if (oldValue != null)
                    oldValue.IsSelected = false;
                _selectedDefinition = value;
                if (_selectedDefinition != null)
                    _selectedDefinition.IsSelected = true;
                OnPropertyChanged();
                SelectedDefinitionChanged?.Invoke(this, new GroupDefinitionSelectionChangedEventArgs(oldValue, value));
            }
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

        public SingleGroupDefinitionSelectionViewModel()
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
