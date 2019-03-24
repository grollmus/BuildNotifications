using System.Collections.Generic;

namespace BuildNotifications.Core.Config
{
    /// <summary>
    /// Represents the data stored for a single connection.
    /// </summary>
    interface IConnectionData
    {
        /// <summary>
        /// Display name of the connection.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Type name of the plugin that is able to construct a provider
        /// for this connection.
        /// </summary>
        string PluginType { get; }

        /// <summary>
        /// Options for this connection.
        /// </summary>
        IReadOnlyDictionary<string,string?> Options { get; }
    }
}