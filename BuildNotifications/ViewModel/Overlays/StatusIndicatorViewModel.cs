using System;
using System.Windows.Input;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Overlays
{
    public class StatusIndicatorViewModel : BaseViewModel
    {
        private UpdateStatus _updateStatus;

        public UpdateStatus UpdateStatus
        {
            get => _updateStatus;
            set
            {
                _updateStatus = value; 
                OnPropertyChanged();
                OnPropertyChanged(nameof(BusyVisible));
                OnPropertyChanged(nameof(ErrorVisible));
            }
        }

        public bool BusyVisible => UpdateStatus == UpdateStatus.Busy;

        public bool ErrorVisible => UpdateStatus == UpdateStatus.Error;

        public ICommand OpenErrorMessageCommand { get; set; }

        public StatusIndicatorViewModel()
        {
            OpenErrorMessageCommand = new DelegateCommand(RequestOpenErrorMessage);
        }

        private void RequestOpenErrorMessage(object obj) => OpenErrorMessageRequested?.Invoke(this, EventArgs.Empty);

        public event EventHandler OpenErrorMessageRequested;
    }
}
