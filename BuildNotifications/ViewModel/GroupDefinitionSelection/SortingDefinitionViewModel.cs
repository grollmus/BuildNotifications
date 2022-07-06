using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection;

public class SortingDefinitionViewModel : BaseViewModel
{
    public SortingDefinitionViewModel(GroupDefinition forDefinition, SortingDefinition sortingDefinition)
    {
        ForDefinition = forDefinition;
        SortingDefinition = sortingDefinition;
    }

    public GroupDefinition ForDefinition { get; }

    public IconType IconType => SortingDefinition.ToIconType();

    public SortingDefinition SortingDefinition { get; set; }
}