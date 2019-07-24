using System;
using System.Collections.Generic;
using System.Linq;
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
            _settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };
        }

        /// <inheritdoc />
        public IUser User { get; }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBaseBuild> FetchAllBuilds()
        {
            var json = await _connection.Query(Constants.Queries.Builds);
            var list = JsonConvert.DeserializeObject<List<Build>>(json, _settings);

            foreach (var build in list)
            {
                if (_knownBuilds.All(b => b.Id != build.Id))
                    _knownBuilds.Add(build);

                yield return build;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBaseBuild> FetchBuildsForDefinition(IBuildDefinition definition)
        {
            var json = await _connection.Query(Constants.Queries.Builds);
            var list = JsonConvert.DeserializeObject<List<Build>>(json, _settings);

            foreach (var build in list)
            {
                if (_knownBuilds.All(b => b.Id != build.Id))
                    _knownBuilds.Add(build);

                if (build.Definition.Equals(definition))
                    yield return build;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBaseBuild> FetchBuildsChangedSince(DateTime date)
        {
            var json = await _connection.Query(Constants.Queries.Builds);
            var list = JsonConvert.DeserializeObject<List<Build>>(json, _settings);

            foreach (var build in list)
            {
                if (_knownBuilds.All(b => b.Id != build.Id))
                    _knownBuilds.Add(build);

                if (!build.LastChangedTime.HasValue || build.LastChangedTime > date)
                    yield return build;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBuildDefinition> FetchExistingBuildDefinitions()
        {
            var json = await _connection.Query(Constants.Queries.Definitions);
            var list = JsonConvert.DeserializeObject<List<BuildDefinition>>(json, _settings);

            foreach (var buildDefinition in list)
            {
                if (_knownBuildDefinitions.All(d => d.Id != buildDefinition.Id))
                    _knownBuildDefinitions.Add(buildDefinition);

                yield return buildDefinition;
            }
        }

        public async IAsyncEnumerable<IBuildDefinition> RemovedBuildDefinitions()
        {
            var json = await _connection.Query(Constants.Queries.Definitions);
            var list = JsonConvert.DeserializeObject<List<BuildDefinition>>(json, _settings);

            var deletedDefinitions = _knownBuildDefinitions.Except(list);

            foreach (var definition in deletedDefinitions)
            {
                yield return definition;
            }
        }

        public async IAsyncEnumerable<IBaseBuild> RemovedBuilds()
        {
            var json = await _connection.Query(Constants.Queries.Builds);
            var list = JsonConvert.DeserializeObject<List<Build>>(json, _settings);

            var deletedBuilds = _knownBuilds.Except(list);

            foreach (var build in deletedBuilds)
            {
                yield return build;
            }
        }

        private readonly List<BuildDefinition> _knownBuildDefinitions = new List<BuildDefinition>();
        private readonly List<Build> _knownBuilds = new List<Build>();

        private readonly Connection _connection;
        private JsonSerializerSettings _settings;
    }
}