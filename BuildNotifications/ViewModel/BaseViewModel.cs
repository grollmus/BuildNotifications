using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Media;
using BuildNotifications.ViewModel.Utils;
using JetBrains.Annotations;

namespace BuildNotifications.ViewModel
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void OnPropertyChanged([CallerMemberName] string propertyName = "");
    }

    public class BaseViewModel : IRemoveTracking, IViewModel
    {
        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected async Task WaitUntilNextFrameIsRenderedAsync()
        {
            var frameRendered = false;

            void FrameRendered(object? sender, EventArgs e)
            {
                frameRendered = true;
            }

            CompositionTarget.Rendering += FrameRendered;

            while (!frameRendered)
            {
                await Task.Delay(100);
            }

            CompositionTarget.Rendering -= FrameRendered;
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