using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Plugin
{
    /// <summary>
    /// Repository containing all loaded plugins.
    /// </summary>
    public interface IPluginRepository
    {
        /// <summary>
        /// List of all loaded build plugins.
        /// </summary>
        IReadOnlyList<IBuildPlugin> Build { get; }

        /// <summary>
        /// List of all loaded source control plugins.
        /// </summary>
        IReadOnlyList<ISourceControlPlugin> SourceControl { get; }
    }
}