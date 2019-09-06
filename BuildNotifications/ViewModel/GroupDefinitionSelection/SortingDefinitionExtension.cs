using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public static class SortingDefinitionExtension
    {
        public static IconType ToIconType(this SortingDefinition sortingDefinition)
        {
            switch (sortingDefinition)
            {
                case SortingDefinition.AlphabeticalDescending:
                    return IconType.AlphabeticalDescending;
                case SortingDefinition.AlphabeticalAscending:
                    return IconType.AlphabeticalAscending;
                case SortingDefinition.StatusAscending:
                    return IconType.StatusAscending;
                case SortingDefinition.StatusDescending:
                    return IconType.StatusDescending;
                case SortingDefinition.DateDescending:
                    return IconType.DateDescending;
                case SortingDefinition.DateAscending:
                    return IconType.DateAscending;
                default:
                    return IconType.None;
            }
        }
    }
}