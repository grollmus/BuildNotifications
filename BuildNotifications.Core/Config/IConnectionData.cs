using System.Collections.Generic;

namespace BuildNotifications.Core.Config
{
    /// <summary>
    /// Represents the data stored for a single connection.
    /// </summary>
    public class ConnectionData
    {
        public ConnectionData()
        {
            Name = string.Empty;
            Options = new Dictionary<string, string?>();
        }

        /// <summary>
        /// Type name of the plugin that is able to construct a build provider
        /// for this connection.
        /// </summary>
        public string? BuildPluginType { get; set; }

        /// <summary>
        /// Display name of the connection.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Options for this connection.
        /// </summary>
        public IReadOnlyDictionary<string, string?> Options { get; set; }

        /// <summary>
        /// Type name of the plugin that is able to construct a branch provider
        /// for this connection.
        /// </summary>
        public string? SourceControlPluginType { get; set; }
    }
}