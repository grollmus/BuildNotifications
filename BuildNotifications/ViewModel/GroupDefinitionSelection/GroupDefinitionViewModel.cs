using System;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Core.Text;
using BuildNotifications.Resources.Icons;
using JetBrains.Annotations;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionViewModel : BaseViewModel
    {
        public GroupDefinitionViewModel(GroupDefinition groupDefinition)
        {
            GroupDefinition = groupDefinition;
            _groupByText = StringLocalizer.GroupBy;
            SortingDefinitionsViewModel = new SortingDefinitionsViewModel(groupDefinition);
            SortingDefinitionsViewModel.SelectedSortingDefinitionChanged += OnSelectedSortingDefinitionChanged;
        }

        [UsedImplicitly]
        public bool DisplaySortingSelection => GroupDefinition != GroupDefinition.None && IsSelected;

        public string GroupByText
        {
            get => _groupByText;
            set
            {
                _groupByText = value;
                OnPropertyChanged();
            }
        }

        public GroupDefinition GroupDefinition { get; }

        public IconType IconType => GroupDefinition.ToIconType();

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public SortingDefinitionsViewModel SortingDefinitionsViewModel { get; set; }

        public event EventHandler<SortingDefinitionsSelectionChangedEventArgs> SelectedSortingDefinitionChanged;

        private void OnSelectedSortingDefinitionChanged(object? sender, SortingDefinitionsSelectionChangedEventArgs e)
        {
            SelectedSortingDefinitionChanged?.Invoke(sender, e);
        }

        private string _groupByText;

        private bool _isSelected;
    }
}