using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsBuildProvider : IBuildProvider
    {
        public TfsBuildProvider(VssConnection connection, string projectId)
        {
            _connection = connection;
            _projectId = projectId;

            //User = new TfsUser(_connection.AuthenticatedIdentity);
        }

        private IBuildDefinition Convert(BuildDefinitionReference definition)
        {
            return new TfsBuildDefinition(definition);
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
        public IAsyncEnumerable<IBuild> FetchAllBuilds()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IBuild> FetchBuildsForDefinition(IBuildDefinition definition)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IBuild> FetchBuildsSince(DateTime date)
        {
            throw new NotImplementedException();
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
}