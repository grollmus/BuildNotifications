using System;
using System.Windows;
using BuildNotifications.PluginInterfaces.Host;

namespace BuildNotifications.ViewModel.Utils
{
    internal class WpfDispatcher : IDispatcher
    {
        public void Dispatch(Action action)
        {
            Application.Current.Dispatcher.BeginInvoke(action);
        }
    }
}