using System.Collections.Generic;
using System.Globalization;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.PluginInterfaces;
using Newtonsoft.Json;

namespace BuildNotifications.Core.Config
{
    internal class Configuration : IConfiguration
    {
        public Configuration()
        {
            Connections = new List<ConnectionData>();
            BuildsToShow = 5;
            UpdateInterval = 30;
            CanceledBuildNotifyConfig = BuildNotificationMode.RequestedByMe;
            FailedBuildNotifyConfig = BuildNotificationMode.RequestedByOrForMe;
            SucceededBuildNotifyConfig = BuildNotificationMode.RequestedByMe;
            PartialSucceededTreatmentMode = PartialSucceededTreatmentMode.TreatAsSucceeded;
            AutoStart = AutostartMode.StartWithWindowsMinimized;
            AnimationSpeed = AnimationMode.Enabled;
            ShowBusyIndicatorOnDeltaUpdates = true;
            IdentitiesOfCurrentUser = new List<IUser>();
            Projects = new List<IProjectConfiguration>();

            Language = "en-US";

            GroupDefinition = new BuildTreeGroupDefinition(
                Pipeline.Tree.Arrangement.GroupDefinition.Source,
                Pipeline.Tree.Arrangement.GroupDefinition.Branch,
                Pipeline.Tree.Arrangement.GroupDefinition.BuildDefinition);

            SortingDefinition = new BuildTreeSortingDefinition(
                Pipeline.Tree.Arrangement.SortingDefinition.AlphabeticalAscending,
                Pipeline.Tree.Arrangement.SortingDefinition.DateDescending,
                Pipeline.Tree.Arrangement.SortingDefinition.AlphabeticalAscending);
        }

        public string Language { get; set; }

        public IBuildTreeGroupDefinition GroupDefinition { get; set; }

        public IBuildTreeSortingDefinition SortingDefinition { get; set; }

        public int BuildsToShow { get; set; }

        public int UpdateInterval { get; set; }

        public bool UsePreReleases { get; set; }

        public BuildNotificationMode CanceledBuildNotifyConfig { get; set; }

        public BuildNotificationMode FailedBuildNotifyConfig { get; set; }

        public BuildNotificationMode SucceededBuildNotifyConfig { get; set; }

        public PartialSucceededTreatmentMode PartialSucceededTreatmentMode { get; set; }

        public AutostartMode AutoStart { get; set; }

        public AnimationMode AnimationSpeed { get; set; }

        public bool ShowBusyIndicatorOnDeltaUpdates { get; set; }

        [JsonIgnore]
        public CultureInfo Culture => CultureInfo.GetCultureInfo(Language);

        public IList<ConnectionData> Connections { get; set; }

        public IList<IProjectConfiguration> Projects { get; set; }

        [JsonIgnore]
        public IList<IUser> IdentitiesOfCurrentUser { get; }
    }
}