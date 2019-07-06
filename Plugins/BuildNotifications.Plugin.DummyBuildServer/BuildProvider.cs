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
            _settings = new JsonSerializerSettings();
            _settings.TypeNameHandling = TypeNameHandling.Auto;
        }

        /// <inheritdoc />
        public IUser User { get; }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBuild> FetchAllBuilds()
        {
            var json = await _connection.Query(Constants.Queries.Builds);
            var list = JsonConvert.DeserializeObject<List<Build>>(json, _settings);

            foreach (var build in list)
            {
                yield return build;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBuild> FetchBuildsForDefinition(IBuildDefinition definition)
        {
            var json = await _connection.Query(Constants.Queries.Builds);
            var list = JsonConvert.DeserializeObject<List<Build>>(json, _settings);

            foreach (var build in list)
            {
                if (build.Definition.Equals(definition))
                {
                    yield return build;
                }
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBuild> FetchBuildsChangedSince(DateTime date)
        {
            var json = await _connection.Query(Constants.Queries.Builds);
            var list = JsonConvert.DeserializeObject<List<Build>>(json, _settings);

            foreach (var build in list)
            {
                if (!build.LastChangedTime.HasValue || build.LastChangedTime > date)
                {
                    yield return build;
                }
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBuildDefinition> FetchExistingBuildDefinitions()
        {
            var json = await _connection.Query(Constants.Queries.Definitions);
            var list = JsonConvert.DeserializeObject<List<BuildDefinition>>(json, _settings);

            foreach (var buildDefinition in list)
            {
                yield return buildDefinition;
            }
        }

        private readonly Connection _connection;
        private JsonSerializerSettings _settings;
    }
}