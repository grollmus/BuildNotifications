using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Pipeline.Cache;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class Pipeline : IPipeline
    {
        public Pipeline(IBuildCombiner combiner)
        {
            _combiner = combiner;
            _buildCache = new PipelineCache<IBuild>();
            _branchCache = new PipelineCache<IBranch>();
            _definitionCache = new PipelineCache<IBuildDefinition>();
        }

        private async Task FetchBranches()
        {
            var providers = _projectList.Select(p => p.BranchProvider).Distinct();

            foreach (var branchProvider in providers)
            {
                var providerId = branchProvider.GetHashCode();

                var branches = branchProvider.FetchExistingBranches();
                await foreach (var branch in branches)
                {
                    var key = new CacheKey(providerId, branch.GetHashCode());
                    _branchCache.AddOrReplace(key, branch);
                }
            }
        }

        private async Task FetchBuildsSinceLastUpdate()
        {
            var providers = _projectList.Select(p => p.BuildProvider).Distinct();

            foreach (var buildProvider in providers)
            {
                var providerId = buildProvider.GetHashCode();

                var builds = buildProvider.FetchBuildsSince(_lastUpdate);
                await foreach (var build in builds)
                {
                    var key = new CacheKey(providerId, build.GetHashCode());
                    _buildCache.AddOrReplace(key, build);
                }
            }

            _lastUpdate = DateTime.Now;
        }

        private async Task FetchDefinitions()
        {
            var providers = _projectList.Select(p => p.BuildProvider).Distinct();

            foreach (var buildProvider in providers)
            {
                var providerId = buildProvider.GetHashCode();

                var definitions = buildProvider.FetchExistingBuildDefinitions();
                await foreach (var definition in definitions)
                {
                    var key = new CacheKey(providerId, definition.GetHashCode());
                    _definitionCache.AddOrReplace(key, definition);
                }
            }
        }

        private async Task InitBuilds()
        {
            var providers = _projectList.Select(p => p.BuildProvider).Distinct();

            foreach (var buildProvider in providers)
            {
                var providerId = buildProvider.GetHashCode();

                var builds = buildProvider.FetchAllBuilds();
                await foreach (var build in builds)
                {
                    var key = new CacheKey(providerId, build.GetHashCode());
                    _buildCache.AddOrReplace(key, build);
                }
            }

            _lastUpdate = DateTime.Now;
        }

        /// <inheritdoc />
        public void AddProject(IProject project)
        {
            _projectList.Add(project);
        }

        /// <inheritdoc />
        public async Task Update()
        {
            var branchTask = FetchBranches();
            var definitionTask = FetchDefinitions();

            await Task.WhenAll(branchTask, definitionTask);

            if (_lastUpdate == DateTime.MinValue)
            {
                await InitBuilds();
            }
            else
            {
                await FetchBuildsSinceLastUpdate();
            }
        }

        private readonly IBuildCombiner _combiner;
        private readonly IPipelineCache<IBuild> _buildCache;
        private readonly IPipelineCache<IBranch> _branchCache;
        private readonly IPipelineCache<IBuildDefinition> _definitionCache;

        private readonly ConcurrentBag<IProject> _projectList = new ConcurrentBag<IProject>();

        private DateTime _lastUpdate = DateTime.MinValue;
    }
}