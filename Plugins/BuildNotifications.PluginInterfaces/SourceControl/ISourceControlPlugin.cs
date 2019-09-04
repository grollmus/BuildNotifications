using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces.Builds;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.SourceControl
{
    /// <summary>
    /// Plugin that allows adding new source control sources.
    /// </summary>
    [PublicAPI]
    public interface ISourceControlPlugin : IPlugin
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
        IBranchProvider? ConstructProvider(object data);

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