using System;
using System.Collections.Generic;
using System.Linq;
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

        private IBuild Enrich(IBaseBuild build)
        {
            return new EnrichedBuild(build, Name);
        }

        public IProjectConfiguration Config { get; set; }

        public string Name { get; set; }

        public async IAsyncEnumerable<IBuild> FetchAllBuilds()
        {
            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var build in buildProvider.FetchAllBuilds())
                {
                    yield return Enrich(build);
                }
            }
        }

        public async IAsyncEnumerable<IBuild> FetchBuildsChangedSince(DateTime lastUpdate)
        {
            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var build in buildProvider.FetchBuildsChangedSince(lastUpdate))
                {
                    yield return Enrich(build);
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
                    yield return Enrich(build);
                }
            }
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
            public EnrichedBuild(IBaseBuild build, string projectName)
            {
                _build = build;
                ProjectName = projectName;
            }

            public bool Equals(IBaseBuild other)
            {
                return _build.Equals(other);
            }

            public string ProjectName { get; }

            public string BranchName => _build.BranchName;

            public IBuildDefinition Definition => _build.Definition;

            public string Id => _build.Id;

            public DateTime? LastChangedTime => _build.LastChangedTime;

            public int Progress => _build.Progress;

            public DateTime? QueueTime => _build.QueueTime;

            public IUser RequestedBy => _build.RequestedBy;

            public IUser? RequestedFor => _build.RequestedFor;

            public BuildStatus Status => _build.Status;

            private readonly IBaseBuild _build;
        }
    }
}