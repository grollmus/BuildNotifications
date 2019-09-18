using System;
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

        /// <summary>
        /// Gets called whenever the instance given by <see cref="SetCurrentConfiguration" /> is changed.
        /// </summary>
        void ConfigurationChanged();

        /// <summary>
        /// Method that returns a complex, serializable object Type. The default configuration of this object should be the default
        /// constructor.
        /// This type is instantiated and used to store any user input within.
        /// </summary>
        /// <returns>
        /// Type of a complex configuration. This configuration is completely plugin specific and may be as big or as
        /// small as the plugin dictates.
        /// </returns>
        /// <remarks>
        /// When the namespaces of this type changes, all previously stored configurations are lost and cannot be
        /// restored. Try to avoid this.
        /// </remarks>
        Type GetConfigurationType();

        /// <summary>
        /// Method used to tell the plugin which configuration instance to use. Within this instance all options that the plugin
        /// needs are stored.
        /// The type of this instance is dictated by <see cref="GetConfigurationType" />
        /// </summary>
        /// <param name="instance">Instance of the current configuration.</param>
        /// <remarks>
        /// The values of this instance may change. Whenever a change happens <see cref="ConfigurationChanged" /> is
        /// called.
        /// </remarks>
        void SetCurrentConfiguration(object instance);
    }
}