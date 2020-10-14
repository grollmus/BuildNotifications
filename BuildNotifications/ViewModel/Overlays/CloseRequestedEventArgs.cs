using System;

namespace BuildNotifications.ViewModel.Overlays
{
    public class CloseRequestedEventArgs : EventArgs
    {
        public bool ShouldHardReload { get; }

        public CloseRequestedEventArgs(bool shouldHardReload)
        {
            ShouldHardReload = shouldHardReload;
        }
    }
}