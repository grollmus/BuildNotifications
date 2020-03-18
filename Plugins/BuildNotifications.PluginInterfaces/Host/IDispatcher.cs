using System;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Host
{
    /// <summary>
    /// 
    /// </summary>
    [PublicAPI]
    public interface IDispatcher
    {
        /// <summary>
        /// Dispatches 
        /// </summary>
        /// <param name="action"></param>
        void Dispatch(Action action);
    }
}