using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Notification;
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
        /// List of all loaded notification processors.
        /// </summary>
        IReadOnlyList<INotificationProcessor> NotificationProcessors { get; }

        /// <summary>
        /// List of all loaded source control plugins.
        /// </summary>
        IReadOnlyList<ISourceControlPlugin> SourceControl { get; }

        /// <summary>
        /// Searches for a build plugin that matches the given typename.
        /// </summary>
        /// <param name="typeName">Type name to search for.</param>
        /// <returns>The build plugin or <c>null</c> if none matches <paramref name="typeName" />.</returns>
        IBuildPlugin? FindBuildPlugin(string? typeName);

        /// <summary>
        /// Searches for any plugin that matches the given typename.
        /// </summary>
        /// <param name="typeName">Type name to search for.</param>
        /// <returns>The plugin IconGeometry or <c>null</c> if none matches <paramref name="typeName" />.</returns>
        string? FindIconGeometry(string typeName);

        /// <summary>
        /// Searches for any plugin that matches the given typename.
        /// </summary>
        /// <param name="typeName">Type name to search for.</param>
        /// <returns>The plugin DisplayName or <c>null</c> if none matches <paramref name="typeName" />.</returns>
        string? FindPluginName(string typeName);

        /// <summary>
        /// Searches for a source control plugin that matches the given typename.
        /// </summary>
        /// <param name="typeName">Type name to search for.</param>
        /// <returns>The source control plugin or <c>null</c> if none matches <paramref name="typeName" />.</returns>
        ISourceControlPlugin? FindSourceControlPlugin(string? typeName);
    }
}