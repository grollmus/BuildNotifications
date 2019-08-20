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
            GroupDefinition = new BuildTreeGroupDefinition(
                Pipeline.Tree.Arrangement.GroupDefinition.Source,
                Pipeline.Tree.Arrangement.GroupDefinition.Branch,
                Pipeline.Tree.Arrangement.GroupDefinition.BuildDefinition);
        }

        [TypesForInstantiation(typeof(BuildTreeGroupDefinition))]
        public IBuildTreeGroupDefinition GroupDefinition { get; set; }

        [MinMax(1, int.MaxValue)]
        public int BuildsToLoadCount { get; set; } = 200;

        [MinMax(1, 10)]
        public int BuildsToShow { get; set; } = 5;

        [MinMax(30, int.MaxValue)]
        public int UpdateInterval { get; set; } = 30;

        public BuildNotificationMode CanceledBuildNotifyConfig { get; set; } = BuildNotificationMode.RequestedByMe;

        public BuildNotificationMode FailedBuildNotifyConfig { get; set; } = BuildNotificationMode.RequestedByMe;

        public BuildNotificationMode SucceededBuildNotifyConfig { get; set; } = BuildNotificationMode.RequestedByMe;

        [CalculatedValues(nameof(PossibleCultures))]
        public CultureInfo Culture { get; set; }

        [TypesForInstantiation(typeof(List<ConnectionData>))]
        public IList<ConnectionData> Connections { get; set; }

        public IEnumerable<string> ConnectionNames() => Connections.Select(x => x.Name);

        [UsedImplicitly]
        public IEnumerable<CultureInfo> PossibleCultures()
        {
            yield return CultureInfo.GetCultureInfo("en-US");
            yield return CultureInfo.GetCultureInfo("de");
        }

        [TypesForInstantiation(typeof(List<IProjectConfiguration>), typeof(ProjectConfiguration))]
        [CalculatedValues(nameof(ConnectionNames), nameof(ConnectionNames))]
        public IList<IProjectConfiguration> Projects { get; set; }
    }
}