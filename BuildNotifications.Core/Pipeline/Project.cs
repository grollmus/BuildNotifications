using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class Project : IProject
    {
        public Project(IEnumerable<IBuildProvider> buildProviders, IEnumerable<IBranchProvider> branchProviders, IProjectConfiguration config)
        {
            Name = config.ProjectName;
            _buildProviders = buildProviders.ToList();
            _branchProviders = branchProviders.ToList();
            Config = config;
        }

        public Project(IBuildProvider buildProvider, IBranchProvider branchProvider, IProjectConfiguration config)
            : this(buildProvider.Yield(), branchProvider.Yield(), config)
        {
        }

        private IBuild Enrich(IBaseBuild build, IBuildProvider buildProvider)
        {
            return new EnrichedBuild(build, Name, buildProvider);
        }

        public IProjectConfiguration Config { get; set; }

        public string Name { get; set; }

        public async IAsyncEnumerable<IBuild> FetchAllBuilds()
        {
            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var build in buildProvider.FetchAllBuilds())
                {
                    yield return Enrich(build, buildProvider);
                }
            }
        }

        public async IAsyncEnumerable<IBuild> FetchBuildsChangedSince(DateTime lastUpdate)
        {
            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var build in buildProvider.FetchBuildsChangedSince(lastUpdate))
                {
                    yield return Enrich(build, buildProvider);
                }
            }
        }

        public async IAsyncEnumerable<IBranch> FetchExistingBranches()
        {
            foreach (var branchProvider in _branchProviders)
            {
                await foreach (var branch in branchProvider.FetchExistingBranches())
                {
                    yield return branch;
                }
            }
        }

        public async IAsyncEnumerable<IBranch> FetchRemovedBranches()
        {
            foreach (var branchProvider in _branchProviders)
            {
                await foreach (var branch in branchProvider.RemovedBranches())
                {
                    yield return branch;
                }
            }
        }

        public async IAsyncEnumerable<IBuild> FetchRemovedBuilds()
        {
            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var build in buildProvider.RemovedBuilds())
                {
                    yield return Enrich(build, buildProvider);
                }
            }
        }

        public async Task UpdateBuilds(IEnumerable<IBuild> builds)
        {
            var enriched = builds.OfType<EnrichedBuild>();
            var grouped = enriched.GroupBy(x => x.Provider);

            var updateTasks = grouped.Select(g => g.Key.UpdateBuilds(g.Select(x => x.OriginalBuild)));
            await Task.WhenAll(updateTasks);
        }

        public IEnumerable<IUser> FetchCurrentUserIdentities()
        {
            return _buildProviders.Select(b => b.User);
        }

        public async IAsyncEnumerable<IBuildDefinition> FetchRemovedBuildDefinitions()
        {
            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var definition in buildProvider.RemovedBuildDefinitions())
                {
                    yield return definition;
                }
            }
        }

        public async IAsyncEnumerable<IBuildDefinition> FetchBuildDefinitions()
        {
            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var definition in buildProvider.FetchExistingBuildDefinitions())
                {
                    yield return definition;
                }
            }
        }

        private readonly List<IBranchProvider> _branchProviders;

        private readonly List<IBuildProvider> _buildProviders;

        private class EnrichedBuild : IBuild
        {
            public EnrichedBuild(IBaseBuild build, string projectName, IBuildProvider provider)
            {
                OriginalBuild = build;
                ProjectName = projectName;
                Provider = provider;
            }

            public IBuildProvider Provider { get; }

            internal IBaseBuild OriginalBuild { get; }

            public bool Equals(IBaseBuild other)
            {
                return OriginalBuild.Equals(other);
            }

            public string ProjectName { get; }

            public string BranchName => OriginalBuild.BranchName;

            public IBuildDefinition Definition => OriginalBuild.Definition;

            public string Id => OriginalBuild.Id;

            public DateTime? LastChangedTime => OriginalBuild.LastChangedTime;

            public int Progress => OriginalBuild.Progress;

            public DateTime? QueueTime => OriginalBuild.QueueTime;

            public IUser RequestedBy => OriginalBuild.RequestedBy;

            public IUser? RequestedFor => OriginalBuild.RequestedFor;

            public BuildStatus Status => OriginalBuild.Status;

            public IBuildLinks Links => OriginalBuild.Links;
        }
    }
}