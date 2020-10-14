namespace BuildNotifications.Core.Config
{
    /// <summary>
    /// Provides access to configuration related services.
    /// </summary>
    public interface IConfigurationService
    {
        /// <summary>
        /// The currently active configuration
        /// </summary>
        IConfiguration CurrentConfig { get; }

        /// <summary>
        /// Serializer to load and save configurations
        /// </summary>
        IConfigurationSerializer Serializer { get; }

        /// <summary>
        /// Merges a new configuration into the currently loaded one.
        /// </summary>
        /// <param name="newConfiguration">Configuration to merge.</param>
        void Merge(IConfiguration newConfiguration);
    }
}