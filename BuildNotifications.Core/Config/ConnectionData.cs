using System.Collections.Generic;
using BuildNotifications.Core.Text;
using ReflectSettings.Attributes;

namespace BuildNotifications.Core.Config
{
    /// <summary>
    /// Represents the data stored for a single connection.
    /// </summary>
    public class ConnectionData
    {
        public ConnectionData()
        {
            Name = StringLocalizer.Instance["New Connection"];
            Options = new Dictionary<string, string?>();
        }

        /// <summary>
        /// Display name of the connection.
        /// </summary>
        [IsDisplayName]
        public string Name { get; set; }

        /// <summary>
        /// Type name of the plugin that is able to construct a build provider
        /// for this connection.
        /// </summary>
        [CalculatedValues(nameof(Configuration.PossibleBuildPlugins))]
        public string? BuildPluginType { get; set; }

        /// <summary>
        /// Type name of the plugin that is able to construct a branch provider
        /// for this connection.
        /// </summary>
        [CalculatedValues(nameof(Configuration.PossibleSourceControlPlugins))]
        public string? SourceControlPluginType { get; set; }

        /// <summary>
        /// Options for this connection.
        /// </summary>
        [TypesForInstantiation(typeof(Dictionary<string, string?>))]
        public IReadOnlyDictionary<string, string?> Options { get; set; }
    }
}