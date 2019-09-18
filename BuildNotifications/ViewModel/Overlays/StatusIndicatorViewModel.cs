using System;
using System.Collections.Generic;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Overlays
{
    public class StatusIndicatorViewModel : BaseViewModel
    {
        public StatusIndicatorViewModel()
        {
            OpenErrorMessageCommand = new DelegateCommand(RequestOpenErrorMessage);
            RequestResumeCommand = new DelegateCommand(RequestResume);
        }

        public bool BusyVisible => UpdateStatus == UpdateStatus.Busy;

        public bool ErrorVisible => UpdateStatus == UpdateStatus.Error;

        public ICommand OpenErrorMessageCommand { get; set; }

        public bool PauseVisible { get; private set; }

        public ICommand RequestResumeCommand { get; set; }

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

        public event EventHandler<OpenErrorRequestEventArgs> OpenErrorMessageRequested;

        public event EventHandler ResumeRequested;

        public void Busy()
        {
            UpdateStatus = UpdateStatus.Busy;
        }

        public void ClearStatus()
        {
            UpdateStatus = UpdateStatus.None;
        }

        public void Error(IEnumerable<INotification> notifications)
        {
            UpdateStatus = UpdateStatus.Error;
            _errorNotifications = notifications;
        }

        public void Pause()
        {
            PauseVisible = true;
            OnPropertyChanged(nameof(PauseVisible));
        }

        public void Resume()
        {
            PauseVisible = false;
            OnPropertyChanged(nameof(PauseVisible));
        }

        private void RequestOpenErrorMessage(object obj)
        {
            if (_errorNotifications == null)
                return;

            OpenErrorMessageRequested?.Invoke(this, new OpenErrorRequestEventArgs(_errorNotifications));
            _errorNotifications = null;

            ClearStatus();
        }

        private void RequestResume(object obj)
        {
            ResumeRequested?.Invoke(this, EventArgs.Empty);
            Resume();
        }

        private UpdateStatus _updateStatus;
        private IEnumerable<INotification>? _errorNotifications;
    }
}