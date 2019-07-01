using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionSortingViewModel : BaseViewModel
    {
        public GroupDefinition ForDefinition { get; }

        public SortingDefinition SortingDefinition { get; set; }

        public IconType IconType => SortingDefinition.ToIconType();

        public GroupDefinitionSortingViewModel(GroupDefinition forDefinition, SortingDefinition sortingDefinition)
        {
            ForDefinition = forDefinition;
            SortingDefinition = sortingDefinition;
        }
    }
}