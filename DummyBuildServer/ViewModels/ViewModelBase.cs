using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace DummyBuildServer.ViewModels
{
    internal abstract class ViewModelBase : INotifyPropertyChanged
    {
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}