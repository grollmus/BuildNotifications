using System.Threading.Tasks;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds
{
    /// <summary>
    /// Plugin that allows adding new build server sources.
    /// </summary>
    [PublicAPI]
    public interface IBuildPlugin : IPlugin
    {
        /// <summary>
        /// Constructs a provider from a set of key-value-pairs.
        /// </summary>
        /// <param name="data">
        /// The configuration instance, which type is specified through
        /// <see cref="IPlugin.GetConfigurationType" />.
        /// </param>
        /// <returns>
        /// The constructed provider. Return <c>null</c> when you are
        /// unable to construct a provider from <paramref name="data" />
        /// </returns>
        IBuildProvider? ConstructProvider(object data);

        /// <summary>
        /// Determines whether a connection can be established with the given configuration.
        /// </summary>
        /// <param name="data">
        /// The configuration instance, which type is specified through
        /// <see cref="IPlugin.GetConfigurationType" />.
        /// </param>
        /// <returns>Result of the connection test.</returns>
        Task<ConnectionTestResult> TestConnection(object data);
    }
}