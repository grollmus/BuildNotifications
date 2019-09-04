using System.Collections.Generic;

namespace BuildNotifications.Core.Config
{
    /// <summary>
    /// (De)serializes configurations from and to a file.
    /// </summary>
    public interface IConfigurationSerializer
    {
        /// <summary>
        /// Load configuration from file. If file does not exist,
        /// default configuration will be returned.
        /// </summary>
        /// <param name="fileName">Path to the file to load from.</param>
        /// <returns>The loaded configuration.</returns>
        IConfiguration Load(string fileName);

        /// <summary>
        /// Loads all predefined connections from a given file.
        /// </summary>
        /// <param name="fileName">Name of the file to load.</param>
        /// <returns>List of all predefined connections.</returns>
        IEnumerable<ConnectionData> LoadPredefinedConnections(string fileName);

        /// <summary>
        /// Saves a configuration to a file.
        /// </summary>
        /// <param name="configuration">Configuration to save.</param>
        /// <param name="fileName">Path to the file to save to.</param>
        void Save(IConfiguration configuration, string fileName);
    }
}