using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Pipeline.Cache;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class Pipeline : IPipeline
    {
        public Pipeline(ITreeBuilder treeBuilder)
        {
            _treeBuilder = treeBuilder;
            _buildCache = new PipelineCache<IBuild>();
            _branchCache = new PipelineCache<IBranch>();
            _definitionCache = new PipelineCache<IBuildDefinition>();

            _pipelineNotifier = new PipelineNotifier();
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
                    var key = new CacheKey(providerId, branch.Name.GetHashCode());
                    _branchCache.AddOrReplace(key, branch);
                }
            }
        }

        private async Task FetchBuilds()
        {
            foreach (var project in _projectList)
            {
                var projectId = project.GetHashCode();

                var builds = _lastUpdate.HasValue
                    ? project.FetchBuildsChangedSince(_lastUpdate.Value)
                    : project.FetchAllBuilds();

                await foreach (var build in builds)
                {
                    var key = new CacheKey(projectId, build.Id.GetHashCode());
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
                    var key = new CacheKey(providerId, definition.Id.GetHashCode());
                    _definitionCache.AddOrReplace(key, definition);
                }
            }
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
            var definitionsTask = FetchDefinitions();
            var buildsTask = FetchBuilds();

            await Task.WhenAll(branchTask, definitionsTask, buildsTask);

            var builds = _buildCache.ContentCopy();
            var branches = _branchCache.ContentCopy();
            var definitions = _definitionCache.ContentCopy();
            var tree = _treeBuilder.Build(builds, branches, definitions);

            _pipelineNotifier.Notify(tree);
        }

        public IPipelineNotifier Notifier => _pipelineNotifier;

        private readonly ITreeBuilder _treeBuilder;
        private readonly IPipelineCache<IBuild> _buildCache;
        private readonly IPipelineCache<IBranch> _branchCache;
        private readonly IPipelineCache<IBuildDefinition> _definitionCache;
        private readonly PipelineNotifier _pipelineNotifier;
        private readonly ConcurrentBag<IProject> _projectList = new ConcurrentBag<IProject>();
        private DateTime? _lastUpdate;
    }
}