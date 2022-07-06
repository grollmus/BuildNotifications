using BuildNotifications.PluginInterfaces.Configuration;
using BuildNotifications.PluginInterfaces.Host;
using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds;

/// <summary>
/// Base interface for plugins.
/// </summary>
[PublicAPI]
public interface IPlugin
{
    /// <summary>
    /// Name for the plugin to display. Not localized.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Geometry path for an vector icon. System.Windows.Media.GeometryDrawing Syntax
    /// </summary>
    string IconSvgPath { get; }

    /// <summary>
    /// Constructs a new configuration instance that can be used to configure the plugin.
    /// </summary>
    IPluginConfiguration ConstructNewConfiguration();

    /// <summary>
    /// Called after the plugin has been loaded.
    /// </summary>
    /// <param name="host">The host that loaded the plugin.</param>
    void OnPluginLoaded(IPluginHost host);
}