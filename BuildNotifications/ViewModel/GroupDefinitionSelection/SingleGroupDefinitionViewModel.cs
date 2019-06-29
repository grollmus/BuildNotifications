using BuildNotifications.Core.Text;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionViewModel : BaseViewModel
    {
        private string _groupByText;
        public Core.Pipeline.Tree.GroupDefinition GroupDefinition { get; }

        public string IconTag { get; set; }

        public IconType IconType => GroupDefinition.ToIconType();

        public string GroupByText
        {
            get => _groupByText;
            set
            {
                _groupByText = value;
                OnPropertyChanged();
            }
        }

        public GroupDefinitionViewModel(Core.Pipeline.Tree.GroupDefinition groupDefinition)
        {
            GroupDefinition = groupDefinition;
            _groupByText = StringLocalizer.Instance["GroupBy"];
        }
    }
}