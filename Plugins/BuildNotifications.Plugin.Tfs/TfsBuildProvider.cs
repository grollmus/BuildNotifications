using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using BuildStatus = Microsoft.TeamFoundation.Build.WebApi.BuildStatus;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsBuildProvider : IBuildProvider
    {
        public TfsBuildProvider(VssConnection connection, Guid projectId)
        {
            _connection = connection;
            _projectId = projectId;

            User = new TfsUser(_connection.AuthenticatedIdentity);
        }

        private int CalculateProgress(Timeline timeLine)
        {
            if (!timeLine.Records.Any())
                return 0;

            var percentagePerStep = 100.0 / timeLine.Records.Count;
            var completedSteps = timeLine.Records.Count(x => x.State == TimelineRecordState.Completed);

            var currentStepFactor = 0.0;
            var currentStep = timeLine.Records.FirstOrDefault(x => x.State == TimelineRecordState.InProgress);
            if (currentStep != null)
                currentStepFactor = (currentStep.PercentComplete ?? 0) / 100.0;

            if (currentStepFactor < 0)
                currentStepFactor = 0;
            if (currentStepFactor > 1)
                currentStepFactor = 1;

            return (int) Math.Round(completedSteps * percentagePerStep + percentagePerStep * currentStepFactor);
        }

        private TfsBuildDefinition Convert(BuildDefinitionReference definition)
        {
            return new TfsBuildDefinition(definition);
        }

        private TfsBuild Convert(Build build)
        {
            return new TfsBuild(build);
        }

        private async Task<TeamProjectReference> GetProject()
        {
            if (_project == null)
            {
                var projectClient = await _connection.GetClientAsync<ProjectHttpClient>();
                var project = await projectClient.GetProject(_projectId.ToString());
                _project = project;
            }

            return _project;
        }

        public IUser User { get; }

        public async IAsyncEnumerable<IBaseBuild> FetchAllBuilds()
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var builds = await buildClient.GetBuildsAsync(project.Id);

            foreach (var build in builds)
            {
                var converted = Convert(build);
                _knownBuilds.Add(converted);
                yield return converted;
            }
        }

        public async IAsyncEnumerable<IBaseBuild> FetchBuildsForDefinition(IBuildDefinition definition)
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            if (!(definition is TfsBuildDefinition tfsDefinition))
            {
                Debug.Fail("Incompatible build definition given");
                yield break;
            }

            var builds = await buildClient.GetBuildsAsync(project.Id, new[] {tfsDefinition.NativeId});
            foreach (var build in builds)
            {
                yield return Convert(build);
            }
        }

        public async IAsyncEnumerable<IBaseBuild> FetchBuildsChangedSince(DateTime date)
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var builds = await buildClient.GetBuildsAsync2(project.Id, minFinishTime: date, queryOrder: BuildQueryOrder.QueueTimeAscending);
            foreach (var build in builds)
            {
                var converted = Convert(build);
                _knownBuilds.Add(converted);
                yield return converted;
            }

            // ReSharper disable BitwiseOperatorOnEnumWithoutFlags
            const BuildStatus statusFilter = BuildStatus.InProgress | BuildStatus.Postponed | BuildStatus.NotStarted | BuildStatus.None;
            // ReSharper restore BitwiseOperatorOnEnumWithoutFlags
            builds = await buildClient.GetBuildsAsync2(project.Id, statusFilter: statusFilter);
            foreach (var build in builds)
            {
                var converted = Convert(build);
                _knownBuilds.Add(converted);
                yield return converted;
            }

            var inProgressBuilds = _knownBuilds.Where(b => b.Status == PluginInterfaces.Builds.BuildStatus.None
                                                           || b.Status == PluginInterfaces.Builds.BuildStatus.Pending
                                                           || b.Status == PluginInterfaces.Builds.BuildStatus.Running).ToList();
            foreach (var strangeBuild in inProgressBuilds)
            {
                var build = await buildClient.GetBuildAsync(project.Id, strangeBuild.BuildId);
                var converted = Convert(build);

                // Replace entry in HashSet so this build won't be updated in the next run
                // if it just completed.
                _knownBuilds.Remove(converted);
                _knownBuilds.Add(converted);

                yield return converted;
            }
        }

        public async IAsyncEnumerable<IBuildDefinition> FetchExistingBuildDefinitions()
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var definitions = await buildClient.GetDefinitionsAsync(project.Id);

            foreach (var definition in definitions)
            {
                var converted = Convert(definition);
                _knownDefinitions.Add(converted);
                yield return converted;
            }
        }

        public async IAsyncEnumerable<IBuildDefinition> RemovedBuildDefinitions()
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var definitions = await buildClient.GetDefinitionsAsync(project.Id);

            var deletedDefinitions = _knownDefinitions.Except(definitions.Select(Convert), new TfsBuildDefinitionComparer());

            foreach (var definition in deletedDefinitions)
            {
                yield return definition;
            }
        }

        public async IAsyncEnumerable<IBaseBuild> RemovedBuilds()
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var builds = await buildClient.GetBuildsAsync(project.Id);

            var deletedBuilds = _knownBuilds.Except(builds.Select(Convert), new TfsBuildComparer());

            foreach (var build in deletedBuilds)
            {
                yield return build;
            }
        }

        public async Task UpdateBuilds(IEnumerable<IBaseBuild> builds)
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var buildList = builds.OfType<TfsBuild>().ToList();

            var timeLines = await Task.WhenAll(buildList.Select(build =>
                buildClient.GetBuildTimelineAsync(project.Id, build.BuildId)));

            for (var i = 0; i < buildList.Count; ++i)
            {
                var build = buildList[i];
                var timeLine = timeLines[i];

                if (timeLine == null || build == null)
                    continue;

                var progress = CalculateProgress(timeLine);
                build.Progress = progress;
            }
        }

        private readonly HashSet<TfsBuildDefinition> _knownDefinitions = new HashSet<TfsBuildDefinition>(new TfsBuildDefinitionComparer());
        private readonly HashSet<TfsBuild> _knownBuilds = new HashSet<TfsBuild>(new TfsBuildComparer());

        private readonly VssConnection _connection;
        private readonly Guid _projectId;
        private TeamProject? _project;
    }
}