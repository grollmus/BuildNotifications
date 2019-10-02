using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Anotar.NLog;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.Core.Plugin;
using BuildNotifications.PluginInterfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;
using ReflectSettings.Attributes;

namespace BuildNotifications.Core.Config
{
    [NoReorder]
    internal class Configuration : IConfiguration
    {
        public Configuration()
        {
            Connections = new List<ConnectionData>();
            Projects = new List<IProjectConfiguration>();

            Language = PossibleLanguages().First();

            GroupDefinition = new BuildTreeGroupDefinition(
                Pipeline.Tree.Arrangement.GroupDefinition.Source,
                Pipeline.Tree.Arrangement.GroupDefinition.Branch,
                Pipeline.Tree.Arrangement.GroupDefinition.BuildDefinition);

            SortingDefinition = new BuildTreeSortingDefinition(
                Pipeline.Tree.Arrangement.SortingDefinition.AlphabeticalAscending,
                Pipeline.Tree.Arrangement.SortingDefinition.DateDescending,
                Pipeline.Tree.Arrangement.SortingDefinition.AlphabeticalAscending);
        }

        [CalculatedValues(nameof(PossibleLanguages))]
        public string Language { get; set; }

        [IgnoredForConfig]
        [JsonIgnore]
        public IPluginRepository? PluginRepository { get; set; }

        [IgnoredForConfig]
        [JsonIgnore]
        public Func<IEnumerable<string?>>? PossibleBuildPluginsFunction { get; set; }

        [IgnoredForConfig]
        [JsonIgnore]
        public Func<IEnumerable<string?>>? PossibleSourceControlPluginsFunction { get; set; }

        public IEnumerable<string> ConnectionNames()
        {
            return Connections.Select(x => x.Name);
        }

        [UsedImplicitly]
        public IEnumerable<string> PossibleLanguages()
        {
            yield return "en-US";
            yield return "de";
        }

        public IBuildTreeGroupDefinition GroupDefinition { get; set; }

        public IBuildTreeSortingDefinition SortingDefinition { get; set; }

        [MinMax(1, 100)]
        public int BuildsToShow { get; set; } = 5;

        [MinMax(30, int.MaxValue)]
        public int UpdateInterval { get; set; } = 30;

        public bool UsePreReleases { get; set; } = false;

        public BuildNotificationMode CanceledBuildNotifyConfig { get; set; } = BuildNotificationMode.RequestedByMe;

        public BuildNotificationMode FailedBuildNotifyConfig { get; set; } = BuildNotificationMode.RequestedByOrForMe;

        public BuildNotificationMode SucceededBuildNotifyConfig { get; set; } = BuildNotificationMode.RequestedByMe;

        public AutostartMode Autostart { get; set; } = AutostartMode.StartWithWindowsMinimized;

        [JsonIgnore]
        public CultureInfo Culture => CultureInfo.GetCultureInfo(Language);

        public IEnumerable<string?> PossibleBuildPlugins()
        {
            return PossibleBuildPluginsFunction?.Invoke() ?? Enumerable.Empty<string?>();
        }

        public IEnumerable<string?> PossibleSourceControlPlugins()
        {
            return PossibleSourceControlPluginsFunction?.Invoke() ?? Enumerable.Empty<string?>();
        }

        public Type BuildPluginConfigurationType(ConnectionData connectionData)
        {
            if (PluginRepository == null || connectionData.BuildPluginType == null)
            {
                LogTo.Debug("PluginRepository not set on Configuration. Impossible to retrieve correct Configuration type for plugin.");
                return typeof(object);
            }

            var type = PluginRepository?.FindConfigurationType(connectionData.BuildPluginType);
            return type ?? typeof(object);
        }

        public Type SourceControlPluginConfigurationType(ConnectionData connectionData)
        {
            if (PluginRepository == null || connectionData.SourceControlPluginType == null)
            {
                LogTo.Debug("PluginRepository not set on Configuration. Impossible to retrieve correct Configuration type for plugin.");
                return typeof(object);
            }

            var type = PluginRepository?.FindConfigurationType(connectionData.SourceControlPluginType);
            return type ?? typeof(object);
        }

        [TypesForInstantiation(typeof(List<ConnectionData>))]
        [CalculatedValues(nameof(PossibleBuildPlugins), nameof(PossibleBuildPlugins))]
        [CalculatedValues(nameof(PossibleSourceControlPlugins), nameof(PossibleSourceControlPlugins))]
        [CalculatedType(nameof(BuildPluginConfigurationType), nameof(BuildPluginConfigurationType))]
        [CalculatedType(nameof(SourceControlPluginConfigurationType), nameof(SourceControlPluginConfigurationType))]
        public IList<ConnectionData> Connections { get; set; }

        [TypesForInstantiation(typeof(List<IProjectConfiguration>), typeof(ProjectConfiguration))]
        [CalculatedValues(nameof(ConnectionNames), nameof(ConnectionNames))]
        public IList<IProjectConfiguration> Projects { get; set; }

        [JsonIgnore]
        [IgnoredForConfig]
        public IList<IUser> IdentitiesOfCurrentUser { get; } = new List<IUser>();
    }
}