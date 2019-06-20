using System.Collections.ObjectModel;

namespace BuildNotifications.ViewModel.Tree
{
    public abstract class BuildTreeNodeViewModel : BaseViewModel
    {
        public ObservableCollection<BuildTreeNodeViewModel> Children { get; }

        public int CurrentTreeLevelDepth { get; set; }

        public int MaxTreeDepth { get; set; }

        protected BuildTreeNodeViewModel()
        {
            Children = new ObservableCollection<BuildTreeNodeViewModel>();
        }
    }
}