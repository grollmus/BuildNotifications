using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Tree
{
    public abstract class BuildTreeNodeViewModel : BaseViewModel, IRemoveTracking
    {
        private bool _isRemoving;
        public RemoveTrackingObservableCollection<BuildTreeNodeViewModel> Children { get; }

        public int CurrentTreeLevelDepth { get; set; }

        public int MaxTreeDepth { get; set; }

        public bool IsRemoving
        {
            get => _isRemoving;
            set
            {
                _isRemoving = value;
                OnPropertyChanged();
            }
        }

        protected BuildTreeNodeViewModel()
        {
            Children = new RemoveTrackingObservableCollection<BuildTreeNodeViewModel>();
        }
    }
}