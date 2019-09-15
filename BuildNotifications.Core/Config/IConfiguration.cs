using System;
using System.Collections.Generic;
using System.Globalization;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.PluginInterfaces;
using Newtonsoft.Json;
using ReflectSettings.Attributes;

namespace BuildNotifications.Core.Config
{
    public interface IConfiguration
    {
        /// <summary>
        /// How many builds initially shall be loaded.
        /// </summary>
        int BuildsToLoadCount { get; }

        /// <summary>
        /// How many builds shall be shown in one group.
        /// </summary>
        int BuildsToShow { get; }

        /// <summary>
        /// For which canceled builds to receive notifications for
        /// </summary>
        BuildNotificationMode CanceledBuildNotifyConfig { get; }

        /// <summary>
        /// List of all saved connections.
        /// </summary>
        IList<ConnectionData> Connections { get; }

        /// <summary>
        /// Culture to use for displaying data.
        /// </summary>
        CultureInfo Culture { get; }

        /// <summary>
        /// For which failed builds to receive notifications for
        /// </summary>
        BuildNotificationMode FailedBuildNotifyConfig { get; }

        /// <summary>
        /// Defines how builds should be grouped.
        /// </summary>
        IBuildTreeGroupDefinition GroupDefinition { get; set; }

        [JsonIgnore]
        [IgnoredForConfig]
        IList<IUser> IdentitiesOfCurrentUser { get; }

        /// <summary>
        /// Language used for localizing the UI.
        /// </summary>
        string Language { get; set; }

        /// <summary>
        /// List of all configured projects.
        /// </summary>
        IList<IProjectConfiguration> Projects { get; }

        /// <summary>
        /// Defines how builds should be sorted.
        /// </summary>
        IBuildTreeSortingDefinition SortingDefinition { get; set; }

        /// <summary>
        /// For which succeeded builds to receive notifications for
        /// </summary>
        BuildNotificationMode SucceededBuildNotifyConfig { get; }

        /// <summary>
        /// Seconds between each update cycle.
        /// </summary>
        int UpdateInterval { get; }

        /// <summary>
        /// Indicates whether to automatically update to releases
        /// marked as PreReleases.
        /// </summary>
        bool UsePreReleases { get; }

        /// <summary>
        /// Retrieves the type of the configuration for the build plugin of the given connection.
        /// </summary>
        Type BuildPluginConfigurationType(ConnectionData connectionData);

        /// <summary>
        /// Retrieves all possible plugin names which provide build provider.
        /// </summary>
        IEnumerable<string?> PossibleBuildPlugins();

        /// <summary>
        /// Retrieves all possible plugin names which provide source control.
        /// </summary>
        IEnumerable<string?> PossibleSourceControlPlugins();

        /// <summary>
        /// Retrieves the type of the configuration for the source control plugin of the given connection.
        /// </summary>
        Type SourceControlPluginConfigurationType(ConnectionData connectionData);
    }
}