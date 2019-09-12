using System;

namespace ToastNotificationsPlugin
{
    internal class ToastEventArgs : EventArgs
    {
        public string Arguments { get; }

        public ToastEventArgs(string arguments)
        {
            Arguments = arguments;
        }
    }
}