using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using JetBrains.Annotations;
using Newtonsoft.Json;
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

        [CalculatedValues(nameof(PossibleLanguages))]
        public string Language { get; set; }

        [IgnoredForConfig]
        [JsonIgnore]
        public Func<IEnumerable<string>> PossibleBuildPluginsFunction { get; set; }

        [IgnoredForConfig]
        [JsonIgnore]
        public Func<IEnumerable<string>> PossibleSourceControlPluginsFunction { get; set; }

        public IEnumerable<string> ConnectionNames()
        {
            return Connections.Select(x => x.Name);
        }

        public IEnumerable<string> PossibleBuildPlugins()
        {
            return PossibleBuildPluginsFunction?.Invoke();
        }

        [UsedImplicitly]
        public IEnumerable<string> PossibleLanguages()
        {
            yield return "en-US";
            yield return "de";
        }

        public IEnumerable<string> PossibleSourceControlPlugins()
        {
            return PossibleSourceControlPluginsFunction?.Invoke();
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

        [JsonIgnore]
        public CultureInfo Culture => CultureInfo.GetCultureInfo(Language);

        [TypesForInstantiation(typeof(List<ConnectionData>))]
        [CalculatedValues(nameof(PossibleBuildPlugins), nameof(PossibleBuildPlugins))]
        [CalculatedValues(nameof(PossibleSourceControlPlugins), nameof(PossibleSourceControlPlugins))]
        public IList<ConnectionData> Connections { get; set; }

        [TypesForInstantiation(typeof(List<IProjectConfiguration>), typeof(ProjectConfiguration))]
        [CalculatedValues(nameof(ConnectionNames), nameof(ConnectionNames))]
        public IList<IProjectConfiguration> Projects { get; set; }
    }
}