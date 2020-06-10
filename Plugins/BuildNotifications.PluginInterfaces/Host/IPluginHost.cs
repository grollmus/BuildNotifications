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

        /// <summary>
        /// Dispatcher for callbacks, which must be handled within the UI thread. E.g. asynchronous operations which set possible values of a combobox.
        /// </summary>
        IDispatcher UiDispatcher { get; }
    }
}