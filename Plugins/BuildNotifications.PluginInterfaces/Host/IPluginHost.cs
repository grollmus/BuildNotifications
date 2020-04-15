using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Host
{
    /// <summary>
    /// Provides access to the application that hosts a plugin.
    /// </summary>
    [PublicAPI]
    public interface IPluginHost
    {
        /// <summary>
        /// The currently running version of the host.
        /// </summary>
        Version Version { get; }
    }
}