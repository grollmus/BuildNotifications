using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionViewModel : IRemoveTracking
    {
        public GroupDefinition GroupDefinition { get; }

        public string IconTag { get; set; }

        public bool IsRemoving { get; set; }

        public IconType IconType => GroupDefinition.ToIconType();

        public GroupDefinitionViewModel(GroupDefinition groupDefinition)
        {
            GroupDefinition = groupDefinition;
        }
    }
}