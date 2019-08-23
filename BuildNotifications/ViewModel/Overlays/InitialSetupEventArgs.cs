using System;

namespace BuildNotifications.ViewModel.Overlays
{
    internal class InitialSetupEventArgs : EventArgs
    {
        public bool ProjectOrConnectionsChanged { get; set; }

        public InitialSetupEventArgs(bool projectOrConnectionsChanged)
        {
            ProjectOrConnectionsChanged = projectOrConnectionsChanged;
        }
    }
}