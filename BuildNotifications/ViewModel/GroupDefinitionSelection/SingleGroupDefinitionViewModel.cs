using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionViewModel : BaseViewModel
    {
        private string _groupByText;
        private bool _isSelected;
        public GroupDefinition GroupDefinition { get; }

        public string IconTag { get; set; }

        public IconType IconType => GroupDefinition.ToIconType();

        public bool DisplaySortingSelection => GroupDefinition != GroupDefinition.None && IsSelected;

        public SortingSelectionViewModel SortingSelectionViewModel { get; set; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public string GroupByText
        {
            get => _groupByText;
            set
            {
                _groupByText = value;
                OnPropertyChanged();
            }
        }

        public GroupDefinitionViewModel(GroupDefinition groupDefinition)
        {
            GroupDefinition = groupDefinition;
            _groupByText = StringLocalizer.Instance["GroupBy"];
            SortingSelectionViewModel = new SortingSelectionViewModel(groupDefinition);
        }
    }
}