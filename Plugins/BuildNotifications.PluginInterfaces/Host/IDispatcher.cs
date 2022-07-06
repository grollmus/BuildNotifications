using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Host;

/// <summary>
/// Provides services for managing the queue of work items for a thread.
/// </summary>
[PublicAPI]
public interface IDispatcher
{
    /// <summary>
    /// Dispatches an action on the main thread of the host.
    /// </summary>
    /// <param name="action">Action to dispatch.</param>
    void Dispatch(Action action);
}