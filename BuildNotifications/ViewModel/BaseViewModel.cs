using System.ComponentModel;
using System.Runtime.CompilerServices;
using BuildNotifications.ViewModel.Utils;
using JetBrains.Annotations;

namespace BuildNotifications.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged, IRemoveTracking
    {
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsRemoving
        {
            get => _isRemoving;
            set
            {
                _isRemoving = value;
                OnPropertyChanged();
            }
        }

        private bool _isRemoving;
    }
}