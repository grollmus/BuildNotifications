using BuildNotifications.PluginInterfaces.Options;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Host
{
    /// <summary>
    /// Provides access to the application that hosts a plugin.
    /// </summary>
    [PublicAPI]
    public interface IPluginHost
    {
        /// <summary>
        /// Factory that can be used to construct option elements.
        /// </summary>
        IOptionSchemaFactory SchemaFactory { get; }
    }
}