using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.GroupDefinitionSelection
{
    public class GroupDefinitionSelectionViewModel
    {
        public RemoveTrackingObservableCollection<GroupDefinitionViewModel> Definitions { get; set; }

        public GroupDefinitionSelectionViewModel()
        {
            Definitions = new RemoveTrackingObservableCollection<GroupDefinitionViewModel>
            {
                new GroupDefinitionViewModel(GroupDefinition.Branch),
                new GroupDefinitionViewModel(GroupDefinition.BuildDefinition),
                new GroupDefinitionViewModel(GroupDefinition.Source),
                new GroupDefinitionViewModel(GroupDefinition.Status),
                new GroupDefinitionViewModel(GroupDefinition.None),
            };
        }
    }
}
