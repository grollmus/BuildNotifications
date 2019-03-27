using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Host;
using BuildNotifications.PluginInterfaces.Options;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.SourceControl
{
    /// <summary>
    /// Plugin that allows adding new source control sources.
    /// </summary>
    [PublicAPI]
    public interface ISourceControlPlugin
    {
        /// <summary>
        /// Constructs a provider from a set of key-value-pairs.
        /// </summary>
        /// <param name="data">Serialized data for the provider.</param>
        /// <returns>
        /// The constructed provider. Return <c>null</c> when you are
        /// unable to construct a provider from <paramref name="data" />
        /// </returns>
        IBranchProvider? ConstructProvider(IReadOnlyDictionary<string, string?> data);

        /// <summary>
        /// The schema that is used when registering new connections to this source.
        /// </summary>
        IOptionSchema GetSchema(IPluginHost host);

        /// <summary>
        /// Serialize a provider to a set of key-value-pairs.
        /// </summary>
        /// <param name="provider">The provider to serialize</param>
        /// <returns>
        /// A set of key-value-pairs that can be used to
        /// construct a provider for the same connection.
        /// </returns>
        IReadOnlyDictionary<string, string?> Serialize(IBuildProvider provider);
    }
}