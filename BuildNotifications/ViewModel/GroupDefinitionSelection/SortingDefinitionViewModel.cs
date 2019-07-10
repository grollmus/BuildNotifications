using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class SortingDefinitionViewModel : BaseViewModel
    {
        public GroupDefinition ForDefinition { get; }

        public SortingDefinition SortingDefinition { get; set; }

        public IconType IconType => SortingDefinition.ToIconType();

        public SortingDefinitionViewModel(GroupDefinition forDefinition, SortingDefinition sortingDefinition)
        {
            ForDefinition = forDefinition;
            SortingDefinition = sortingDefinition;
        }
    }
}