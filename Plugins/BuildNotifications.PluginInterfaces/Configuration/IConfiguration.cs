using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Configuration.Options;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Configuration
{
    /// <summary>
    /// Provides access to the configuration of a plugin.
    /// </summary>
    [PublicAPI]
    public interface IConfiguration
    {
        /// <summary>
        /// The localizer that can be used to retrieve localized texts for this option.
        /// </summary>
        ILocalizer Localizer { get; }

        /// <summary>
        /// Called when configuration is loaded from persistance.
        /// </summary>
        /// <param name="serialized">Previously serialized content to read configuration from.</param>
        /// <returns><c>true</c> when deserialization was successful; otherwise <c>false</c>.</returns>
        bool Deserialize(string serialized);

        /// <summary>
        /// List all options that are contained in this configuration.
        /// </summary>
        /// <returns>List of all available options.</returns>
        IEnumerable<IOption> ListAvailableOptions();

        /// <summary>
        /// Called when configuration is being persisted.
        /// </summary>
        /// <returns>A string containing all necessary information to persist this configuration.</returns>
        string Serialize();
    }
}