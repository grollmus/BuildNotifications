using System.Collections.Generic;

namespace BuildNotifications.Core.Plugin;

/// <summary>
/// Loader that searches for and loads plugins.
/// </summary>
public interface IPluginLoader
{
    /// <summary>
    /// Creates a plugin repository that contains all plugins that could
    /// be loaded from DLLs found in the given folders.
    /// </summary>
    /// <param name="folders">List of folders to search for plugins.</param>
    /// <returns>The created repository.</returns>
    /// <remarks>
    /// Folders contained <paramref name="folders" /> are considered
    /// top level folders. I.e. they will not be searched but only folders
    /// contained this folder.
    /// </remarks>
    IPluginRepository LoadPlugins(IEnumerable<string> folders);
}