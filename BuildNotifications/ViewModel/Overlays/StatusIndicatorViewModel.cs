using System;
using System.Collections.Generic;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Overlays
{
    public class StatusIndicatorViewModel : BaseViewModel
    {
        private UpdateStatus _updateStatus;
        private IEnumerable<INotification>? _errorNotifications;

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

        public bool PauseVisible => _isPaused;

        private bool _isPaused;

        public void Pause()
        {
            _isPaused = true;
            OnPropertyChanged(nameof(PauseVisible));
        }

        public void Resume()
        {
            _isPaused = false;
            OnPropertyChanged(nameof(PauseVisible));
        }

        public void Busy()
        {
            UpdateStatus = UpdateStatus.Busy;
        }

        public void Error(IEnumerable<INotification> notifications)
        {
            UpdateStatus = UpdateStatus.Error;
            _errorNotifications = notifications;
        }

        public void ClearStatus()
        {
            UpdateStatus = UpdateStatus.None;
        }

        public ICommand OpenErrorMessageCommand { get; set; }

        public ICommand RequestResumeCommand { get; set; }

        public StatusIndicatorViewModel()
        {
            OpenErrorMessageCommand = new DelegateCommand(RequestOpenErrorMessage);
            RequestResumeCommand = new DelegateCommand(RequestResume);
        }

        private void RequestResume(object obj)
        {
            ResumeRequested?.Invoke(this, EventArgs.Empty);
            Resume();
        }

        private void RequestOpenErrorMessage(object obj)
        {
            if (_errorNotifications == null)
                return;

            OpenErrorMessageRequested?.Invoke(this, new OpenErrorRequestEventArgs(_errorNotifications));
            _errorNotifications = null;

            ClearStatus();
        }

        public event EventHandler<OpenErrorRequestEventArgs> OpenErrorMessageRequested;

        public event EventHandler ResumeRequested;
    }
}
