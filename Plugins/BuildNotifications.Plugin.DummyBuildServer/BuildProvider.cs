using System;
using System.Collections.Generic;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using Newtonsoft.Json;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class BuildProvider : IBuildProvider
    {
        public BuildProvider(Connection connection)
        {
            _connection = connection;
        }

        /// <inheritdoc />
        public IUser User { get; }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBuild> FetchAllBuilds()
        {
            var json = await _connection.Query(Constants.Queries.Definitions);

            foreach (var build in JsonConvert.DeserializeObject<List<Build>>(json))
            {
                yield return build;
            }
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IBuild> FetchBuildsForDefinition(IBuildDefinition definition)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IBuild> FetchBuildsStartedSince(DateTime date)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBuildDefinition> FetchExistingBuildDefinitions()
        {
            var json = await _connection.Query(Constants.Queries.Definitions);

            foreach (var buildDefinition in JsonConvert.DeserializeObject<List<BuildDefinition>>(json))
            {
                yield return buildDefinition;
            }
        }

        private readonly Connection _connection;
    }
}