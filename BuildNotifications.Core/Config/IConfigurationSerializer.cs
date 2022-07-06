namespace BuildNotifications.Core.Config;

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
    /// <param name="success">Indicates whether the import was successful or not.</param>
    /// <returns>The loaded configuration.</returns>
    IConfiguration Load(string fileName, out bool success);

    /// <summary>
    /// Saves a configuration to a file.
    /// </summary>
    /// <param name="configuration">Configuration to save.</param>
    /// <param name="fileName">Path to the file to save to.</param>
    bool Save(IConfiguration configuration, string fileName);
}