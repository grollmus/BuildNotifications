using System.ComponentModel;
using System.Runtime.CompilerServices;
using BuildNotifications.ViewModel.Utils;
using JetBrains.Annotations;

namespace BuildNotifications.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged, IRemoveTracking
    {
        private bool _isBusy;
        private bool _isRemoving;
        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsRemoving
        {
            get => _isRemoving;
            set
            {
                _isRemoving = value;
                OnPropertyChanged();
            }
        }
    }
}