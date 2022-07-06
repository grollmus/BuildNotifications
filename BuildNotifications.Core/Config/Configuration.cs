using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;

namespace BuildNotifications.Core.Config;

internal class Configuration : IConfiguration
{
    public string Language { get; set; } = "en-US";

    public IBuildTreeGroupDefinition GroupDefinition { get; set; } = new BuildTreeGroupDefinition(
        Pipeline.Tree.Arrangement.GroupDefinition.Source,
        Pipeline.Tree.Arrangement.GroupDefinition.Branch,
        Pipeline.Tree.Arrangement.GroupDefinition.BuildDefinition);

    public IBuildTreeSortingDefinition SortingDefinition { get; set; } = new BuildTreeSortingDefinition(
        Pipeline.Tree.Arrangement.SortingDefinition.AlphabeticalAscending,
        Pipeline.Tree.Arrangement.SortingDefinition.DateDescending,
        Pipeline.Tree.Arrangement.SortingDefinition.AlphabeticalAscending);

    public int BuildsToShow { get; set; } = 5;

    public int UpdateInterval { get; set; } = 30;

    public bool UsePreReleases { get; set; }

    public Theme ApplicationTheme { get; set; } = Theme.Dark;

    public BuildNotificationModes CanceledBuildNotifyConfig { get; set; } = BuildNotificationModes.RequestedByMe;

    public BuildNotificationModes FailedBuildNotifyConfig { get; set; } = BuildNotificationModes.RequestedByOrForMe;

    public BuildNotificationModes SucceededBuildNotifyConfig { get; set; } = BuildNotificationModes.RequestedByMe;

    public PartialSucceededTreatmentMode PartialSucceededTreatmentMode { get; set; } = PartialSucceededTreatmentMode.TreatAsSucceeded;

    public AutostartMode Autostart { get; set; } = AutostartMode.StartWithWindowsMinimized;

    public AnimationMode AnimationSpeed { get; set; } = AnimationMode.Enabled;

    public bool ShowBusyIndicatorOnDeltaUpdates { get; set; } = true;

    public IList<ConnectionData> Connections { get; set; } = new List<ConnectionData>();

    public IList<IProjectConfiguration> Projects { get; set; } = new List<IProjectConfiguration>();
}