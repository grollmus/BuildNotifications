using System;
using System.Reflection;
using BuildNotifications.PluginInterfaces.Host;

namespace BuildNotifications.Core.Plugin.Host
{
    internal class PluginHost : IPluginHost
    {
        public Version Version => Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0);
    }
}