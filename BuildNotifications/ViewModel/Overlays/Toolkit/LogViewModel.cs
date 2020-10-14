using System.Windows;
using BuildNotifications.Core.Toolkit;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Overlays.Toolkit
{
    internal class LogViewModel : BaseViewModel
    {
        public RemoveTrackingObservableCollection<LogEventInfoViewModel> Entries { get; } = new RemoveTrackingObservableCollection<LogEventInfoViewModel>();

        private bool _hasLogs;

        public bool HasLogs
        {
            get => _hasLogs;
            set
            {
                var changed = _hasLogs != value;
                _hasLogs = value;

                if (changed)
                    OnPropertyChanged();
            }
        }

        public LogViewModel(IPluginTestAction action)
        {
            Entries.SortDescending(l => l.Timestamp);
            Entries.CollectionChanged += (sender, args) => HasLogs = Entries.Count > 0;

            action.LogEventReceived += (sender, args) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var logEventInfoViewModel = new LogEventInfoViewModel(args.LogEventInfo);
                    Entries.Add(logEventInfoViewModel);
                });
            };
        }
    }
}