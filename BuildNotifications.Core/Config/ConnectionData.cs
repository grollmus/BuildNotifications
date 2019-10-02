﻿using BuildNotifications.Core.Text;
using JetBrains.Annotations;
using ReflectSettings.Attributes;

namespace BuildNotifications.Core.Config
{
    /// <summary>
    /// Represents the data stored for a single connection.
    /// </summary>
    [NoReorder]
    public class ConnectionData
    {
        public ConnectionData()
        {
            Name = StringLocalizer.NewConnection;
        }

        /// <summary>
        /// Display name of the connection.
        /// </summary>
        [IgnoredForConfig]
        public string Name { get; set; }

        /// <summary>
        /// Type name of the plugin that is able to construct a build provider
        /// for this connection.
        /// </summary>
        [CalculatedValues(nameof(Configuration.PossibleBuildPlugins))]
        public string? BuildPluginType { get; set; }

        /// <summary>
        /// Configuration instance for the selected BuildPlugin to use
        /// </summary>
        [CalculatedType(nameof(Configuration.BuildPluginConfigurationType))]
        public object? BuildPluginConfiguration { get; set; }

        /// <summary>
        /// Type name of the plugin that is able to construct a branch provider
        /// for this connection.
        /// </summary>
        [CalculatedValues(nameof(Configuration.PossibleSourceControlPlugins))]
        public string? SourceControlPluginType { get; set; }

        /// <summary>
        /// Configuration instance for the selected SourceControlPlugin to use
        /// </summary>
        [CalculatedType(nameof(Configuration.SourceControlPluginConfigurationType))]
        public object? SourceControlPluginConfiguration { get; set; }
    }
}