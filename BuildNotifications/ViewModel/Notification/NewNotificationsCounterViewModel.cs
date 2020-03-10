using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.ViewModel.Notification
{
    public class NewNotificationsCounterViewModel : BaseViewModel
    {
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

        public bool CountIsZero => Count == 0;

        public string CountToDisplay => Count > 9 ? "*" : Count.ToString(StringLocalizer.CurrentCulture);

        public BuildStatus HighestStatus
        {
            get => _highestStatus;
            set
            {
                _highestStatus = value;
                OnPropertyChanged();
            }
        }

        private int _count;

        private BuildStatus _highestStatus;
    }
}