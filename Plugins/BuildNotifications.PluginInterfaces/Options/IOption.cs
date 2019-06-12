using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Options
{
    /// <summary>
    /// Defines an option in a schema.
    /// </summary>
    [PublicAPI]
    public interface IOption
    {
        /// <summary>
        /// A detailed description of the option.
        /// </summary>
        string? Description { get; set; }

        /// <summary>
        /// Id of this option.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// Name of the option.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Indicates whether this option must be filled by the user.
        /// </summary>
        bool Required { get; set; }
    }
}