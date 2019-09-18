using System;

namespace BuildNotifications.ViewModel.Overlays
{
    internal class InitialSetupEventArgs : EventArgs
    {
        public InitialSetupEventArgs(bool projectOrConnectionsChanged)
        {
            ProjectOrConnectionsChanged = projectOrConnectionsChanged;
        }

        public bool ProjectOrConnectionsChanged { get; set; }
    }
}