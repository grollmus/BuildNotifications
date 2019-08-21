using JetBrains.Annotations;

namespace BuildNotifications.PluginInterfaces.Builds
{
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
    }
}