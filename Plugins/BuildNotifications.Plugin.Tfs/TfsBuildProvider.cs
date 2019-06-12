using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using BuildStatus = BuildNotifications.PluginInterfaces.Builds.BuildStatus;

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

        private IBuild Convert(Build build)
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
        public async IAsyncEnumerable<IBuild> FetchAllBuilds()
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
        public async IAsyncEnumerable<IBuild> FetchBuildsForDefinition(IBuildDefinition definition)
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var tfsDefinition = definition as TfsBuildDefinition;

            if (tfsDefinition == null)
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
        public async IAsyncEnumerable<IBuild> FetchBuildsSince(DateTime date)
        {
            var project = await GetProject();
            var buildClient = await _connection.GetClientAsync<BuildHttpClient>();

            var builds = await buildClient.GetBuildsAsync(project.Id, null, null, null, date);
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

        private readonly VssConnection _connection;
        private readonly string _projectId;
        private TeamProject? _project;
    }

    internal class TfsBuild : IBuild
    {
        public TfsBuild(Build build)
        {
            _id = build.Url;
            Id = build.Id.ToString();

            QueueTime = build.QueueTime;
            BranchName = build.SourceBranch;

            _nativeResult = build.Result;
            _nativeStatus = build.Status;

            RequestedBy = new TfsUser(build.RequestedBy);
            RequestedFor = new TfsUser(build.RequestedFor);
            Definition = new TfsBuildDefinition(build.Definition);
        }

        /// <inheritdoc />
        public bool Equals(IBuild other)
        {
            return _id == (other as TfsBuild)?._id;
        }

        /// <inheritdoc />
        public string BranchName { get; }

        /// <inheritdoc />
        public IBuildDefinition Definition { get; }

        /// <inheritdoc />
        public string Id { get; }

        /// <inheritdoc />
        public DateTime? LastChangedTime { get; }

        /// <inheritdoc />
        public DateTime? QueueTime { get; }

        /// <inheritdoc />
        public IUser RequestedBy { get; }

        /// <inheritdoc />
        public IUser? RequestedFor { get; }

        /// <inheritdoc />
        public BuildStatus Status
        {
            get
            {
                if (!_nativeStatus.HasValue)
                {
                    return BuildStatus.None;
                }

                switch (_nativeStatus.Value)
                {
                    case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Cancelling:
                        return BuildStatus.Cancelled;

                    case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress:
                    case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted:
                    case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Postponed:
                        return BuildStatus.Pending;
                }

                if (!_nativeResult.HasValue)
                {
                    return BuildStatus.Pending;
                }

                switch (_nativeResult.Value)
                {
                    case BuildResult.Canceled:
                        return BuildStatus.Cancelled;
                    case BuildResult.Failed:
                        return BuildStatus.Failed;
                    case BuildResult.PartiallySucceeded:
                        return BuildStatus.PartiallySucceeded;
                    case BuildResult.Succeeded:
                        return BuildStatus.Succeeded;
                }

                return BuildStatus.None;
            }
        }

        private string _id;
        private Microsoft.TeamFoundation.Build.WebApi.BuildStatus? _nativeStatus;
        private BuildResult? _nativeResult;
    }
}