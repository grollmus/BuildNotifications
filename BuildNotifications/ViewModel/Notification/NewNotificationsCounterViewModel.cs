using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.ViewModel.Notification
{
    public class NewNotificationsCounterViewModel : BaseViewModel
    {
        private int _count;

        public int Count
        {
            get => _count;
            set
            {
                _count = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CountIsZero));
                OnPropertyChanged(nameof(CountToDisplay));
            }
        }

        private BuildStatus _highestStatus;

        public BuildStatus HighestStatus
        {
            get => _highestStatus;
            set
            {
                _highestStatus = value;
                OnPropertyChanged();
            }
        }

        public string CountToDisplay => Count > 9 ? "*" : Count.ToString();

        public bool CountIsZero => Count == 0;
    }
}