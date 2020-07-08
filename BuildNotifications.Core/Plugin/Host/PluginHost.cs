using System;
using System.Reflection;
using BuildNotifications.PluginInterfaces.Host;

namespace BuildNotifications.Core.Plugin.Host
{
    internal class PluginHost : IPluginHost
    {
        public PluginHost(IDispatcher uiDispatcher)
        {
            UiDispatcher = uiDispatcher;
        }

        public Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0);

        public IDispatcher UiDispatcher { get; }
    }
}