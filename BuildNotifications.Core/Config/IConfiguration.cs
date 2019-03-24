using System.Collections.Generic;

namespace BuildNotifications.Core.Config
{
    internal interface IConfiguration
    {
        /// <summary>
        /// List of all saved connections.
        /// </summary>
        IList<IConnectionData> Connections { get; }
    }
}