using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using JetBrains.Annotations;
using ReflectSettings.Attributes;

namespace BuildNotifications.Core.Config
{
    internal class Configuration : IConfiguration
    {
        public Configuration()
        {
            Projects = new List<IProjectConfiguration>();
            Connections = new List<ConnectionData>();

            BuildsToLoadCount = 200;
            BuildsToShow = 5;
            UpdateInterval = 30;
            CanceledBuildNotifyConfig = BuildNotificationMode.RequestedByMe;
            SucceededBuildNotifyConfig = BuildNotificationMode.RequestedByMe;
            FailedBuildNotifyConfig = BuildNotificationMode.RequestedByOrForMe;

            GroupDefinition = new BuildTreeGroupDefinition(
                Pipeline.Tree.Arrangement.GroupDefinition.Source,
                Pipeline.Tree.Arrangement.GroupDefinition.Branch,
                Pipeline.Tree.Arrangement.GroupDefinition.BuildDefinition);

            Culture = CultureInfo.GetCultureInfo("en-US");
        }

        [MinMax(1, int.MaxValue)]
        public int BuildsToLoadCount { get; set; }

        [MinMax(1, 10)]
        public int BuildsToShow { get; set; }

        public BuildNotificationMode CanceledBuildNotifyConfig { get; set; }

        [TypesForInstantiation(typeof(List<ConnectionData>))]
        public IList<ConnectionData> Connections { get; set; }

        public IEnumerable<string> ConnectionNames()
        {
            return Connections.Select(x => x.Name);
        }

        [CalculatedValues(nameof(PossibleCultures))]
        public CultureInfo Culture { get; set; }

        [UsedImplicitly]
        public IEnumerable<CultureInfo> PossibleCultures()
        {
            yield return CultureInfo.GetCultureInfo("en-US");
            yield return CultureInfo.GetCultureInfo("de");
        }

        [TypesForInstantiation(typeof(List<IProjectConfiguration>), typeof(ProjectConfiguration))]
        [CalculatedValues(nameof(ConnectionNames), nameof(ConnectionNames))]
        public IList<IProjectConfiguration> Projects { get; set; }

        public BuildNotificationMode FailedBuildNotifyConfig { get; set; }

        public BuildNotificationMode SucceededBuildNotifyConfig { get; set; }

        [TypesForInstantiation(typeof(BuildTreeGroupDefinition))]
        public IBuildTreeGroupDefinition GroupDefinition { get; set; }

        [MinMax(30, int.MaxValue)]
        public int UpdateInterval { get; set; }
    }
}