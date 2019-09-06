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

        /// <summary>
        /// Defines how builds should be sorted.
        /// </summary>
        IBuildTreeSortingDefinition SortingDefinition { get; set; }

        /// <summary>
        /// List of all configured projects.
        /// </summary>
        IList<IProjectConfiguration> Projects { get; }

        /// <summary>
        /// For which succeeded builds to receive notifications for
        /// </summary>
        BuildNotificationMode SucceededBuildNotifyConfig { get; }

        /// <summary>
        /// Seconds between each update cycle.
        /// </summary>
        int UpdateInterval { get; }

        [JsonIgnore]
        [IgnoredForConfig]
        IList<IUser> IdentitiesOfCurrentUser { get; }
    }
}