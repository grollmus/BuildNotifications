using System.Collections.Generic;

namespace BuildNotifications.Core;

/// <summary>
/// Contains paths to files and folders.
/// </summary>
public interface IPathResolver
{
    /// <summary>
    /// Full path to folder where configuration files are stored.
    /// </summary>
    string ConfigurationFolder { get; }

    /// <summary>
    /// List of full paths to directories where to search for plugins.
    /// </summary>
    IEnumerable<string> PluginFolders { get; }

    /// <summary>
    /// Name of the file that contains user configuration.
    /// </summary>
    string UserConfigurationFileName { get; }

    /// <summary>
    /// Full path to the file that contains user configuration.
    /// </summary>
    string UserConfigurationFilePath { get; }
}