using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.Core.Config;

public interface IConfiguration
{
    /// <summary>
    /// Determines the speed of animations within the app
    /// </summary>
    AnimationMode AnimationSpeed { get; set; }

    /// <summary>
    /// Application theme to use.
    /// </summary>
    Theme ApplicationTheme { get; set; }

    /// <summary>
    /// Whether BuildNotifications shall start with Windows
    /// </summary>
    AutostartMode Autostart { get; set; }

    /// <summary>
    /// How many builds shall be shown in one group.
    /// </summary>
    int BuildsToShow { get; set; }

    /// <summary>
    /// For which canceled builds to receive notifications for
    /// </summary>
    BuildNotificationModes CanceledBuildNotifyConfig { get; set; }

    /// <summary>
    /// List of all saved connections.
    /// </summary>
    IList<ConnectionData> Connections { get; }

    /// <summary>
    /// For which failed builds to receive notifications for
    /// </summary>
    BuildNotificationModes FailedBuildNotifyConfig { get; set; }

    /// <summary>
    /// Defines how builds should be grouped.
    /// </summary>
    IBuildTreeGroupDefinition GroupDefinition { get; set; }

    /// <summary>
    /// Language used for localizing the UI.
    /// </summary>
    string Language { get; set; }

    /// <summary>
    /// How to treat builds with warnings (or partially succeeded builds)
    /// </summary>
    PartialSucceededTreatmentMode PartialSucceededTreatmentMode { get; set; }

    /// <summary>
    /// List of all configured projects.
    /// </summary>
    IList<IProjectConfiguration> Projects { get; }

    /// <summary>
    /// Whether to show the busy indicator when fetching delta updates
    /// </summary>
    bool ShowBusyIndicatorOnDeltaUpdates { get; set; }

    /// <summary>
    /// Defines how builds should be sorted.
    /// </summary>
    IBuildTreeSortingDefinition SortingDefinition { get; set; }

    /// <summary>
    /// For which succeeded builds to receive notifications for
    /// </summary>
    BuildNotificationModes SucceededBuildNotifyConfig { get; set; }

    /// <summary>
    /// Seconds between each update cycle.
    /// </summary>
    int UpdateInterval { get; set; }

    /// <summary>
    /// Indicates whether to automatically update to releases
    /// marked as PreReleases.
    /// </summary>
    bool UsePreReleases { get; set; }
}