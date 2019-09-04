using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public TfsBuildProvider(VssConnection connection, string projectId)
        {
            _connection = connection;
            _projectId = projectId;

            User = new TfsUser(_connection.AuthenticatedIdentity);
        }

        private IBuildDefinition Convert(BuildDefinitionReference definition)
        {
            return new TfsBuildDefinition(definition);
        }

        private IBaseBuild Convert(Build build)
        {
            return new TfsBuild(build);
        }

        private async Task<TeamProjectReference> GetProject()
        {
            if (_project == null)
            {
                var projectClient = await _connection.GetClientAsync<ProjectHttpClient>();
                var project = await projectClient.GetProject(_projectId);
                _project = project;
            }

            return _project;
        }

        /// <inheritdoc />
        public IUser User { get; }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBaseBuild> FetchAllBuilds()
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var builds = await buildClient.GetBuildsAsync(project.Id);

            foreach (var build in builds)
            {
                yield return Convert(build);
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async IAsyncEnumerable<IBaseBuild> FetchBuildsChangedSince(DateTime date)
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var builds = await buildClient.GetBuildsAsync2(project.Id, minFinishTime: date, queryOrder: BuildQueryOrder.QueueTimeAscending);
            foreach (var build in builds)
            {
                yield return Convert(build);
            }

            builds = await buildClient.GetBuildsAsync2(project.Id, statusFilter: BuildStatus.InProgress);
            foreach (var build in builds)
            {
                yield return Convert(build);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBuildDefinition> FetchExistingBuildDefinitions()
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var definitions = await buildClient.GetDefinitionsAsync(project.Id);

            foreach (var definition in definitions)
            {
                yield return Convert(definition);
            }
        }

        public async IAsyncEnumerable<IBuildDefinition> RemovedBuildDefinitions()
        {
            await Task.CompletedTask;

            yield break;
        }

        public async IAsyncEnumerable<IBaseBuild> RemovedBuilds()
        {
            await Task.CompletedTask;

            yield break;
        }

        private readonly VssConnection _connection;
        private readonly string _projectId;
        private TeamProject? _project;
    }
}