using System;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.ViewModel.Overlays
{
    public abstract class OverlayViewModel : BaseViewModel
    {
        private double _opacity;

        public event EventHandler<CloseRequestedEventArgs>? CloseRequested;

        public double Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                OnPropertyChanged();
            }
        }

        protected OverlayViewModel()
        {
            App.GlobalTweenHandler.Add(this.Tween(x => x.Opacity).To(1.0).In(0.5).Ease(Easing.ExpoEaseOut));
        }

        protected void RequestClose(bool shouldHardReload) => CloseRequested?.Invoke(this, new CloseRequestedEventArgs(shouldHardReload));
    }
}