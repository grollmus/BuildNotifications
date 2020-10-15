using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;
using NLog.Fluent;

namespace BuildNotifications.Core.Pipeline
{
    internal class Project : IProject
    {
        public Project(IEnumerable<IBuildProvider> buildProviders, IBranchProvider? branchProvider, IProjectConfiguration projectConfig, IConfiguration applicationConfig)
        {
            Name = projectConfig.ProjectName;
            _buildProviders = buildProviders.ToList();
            _branchProvider = branchProvider;
            _applicationConfig = applicationConfig;
            Config = projectConfig;

            _buildFilter = new ListBuildFilter(projectConfig, BranchNameExtractor);
        }

        public Project(IBuildProvider buildProvider, IBranchProvider branchProvider, IProjectConfiguration config, IConfiguration configuration)
            : this(buildProvider.Yield(), branchProvider, config, configuration)
        {
        }

        private IBranchNameExtractor BranchNameExtractor => _branchProvider?.NameExtractor ?? new NullBranchNameExtractor();

        private IBuild Enrich(IBaseBuild build, IBuildProvider buildProvider) => new EnrichedBuild(build, Name, Guid, buildProvider);

        private string ExtractBranchName(IPullRequest pr)
        {
            switch (Config.PullRequestDisplay)
            {
                case PullRequestDisplayMode.Name:
                    return pr.Description;
                case PullRequestDisplayMode.Path:
                    var sourceName = BranchNameExtractor.ExtractDisplayName(pr.SourceBranch);
                    var targetName = BranchNameExtractor.ExtractDisplayName(pr.TargetBranch);
                    return $"{sourceName} into {targetName}";

                default:
                    return $"PR {pr.Id}";
            }
        }

        private bool IsAllowed(IBaseBuild build) => _buildFilter.IsAllowed(build);

        public IProjectConfiguration Config { get; set; }

        public string Name { get; set; }

        public Guid Guid { get; } = Guid.NewGuid();

        public async IAsyncEnumerable<IBuild> FetchAllBuilds()
        {
            _buildFilter.InitializeStringMatcher(BranchNameExtractor);
            var buildsPerGroup = _applicationConfig.BuildsToShow * _branchProvider?.ExistingBranchCount ?? 1;

            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var build in buildProvider.FetchAllBuilds(buildsPerGroup))
                {
                    if (IsAllowed(build))
                        yield return Enrich(build, buildProvider);
                    else
                        Log.Debug().Message($"Build {build.Definition.Name}.{build.Id} on {build.BranchName} fetched but was filtered").Write();
                }
            }
        }

        public async IAsyncEnumerable<IBuild> FetchBuildsChangedSince(DateTime lastUpdate)
        {
            _buildFilter.InitializeStringMatcher(BranchNameExtractor);

            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var build in buildProvider.FetchBuildsChangedSince(lastUpdate))
                {
                    if (IsAllowed(build))
                        yield return Enrich(build, buildProvider);
                    else
                        Log.Debug().Message($"Build {build.Definition.Name}.{build.Id} on {build.BranchName} fetched but was filtered").Write();
                }
            }
        }

        public async IAsyncEnumerable<IBranch> FetchExistingBranches()
        {
            if (_branchProvider == null)
                yield break;

            await foreach (var branch in _branchProvider.FetchExistingBranches())
            {
                if (Config.PullRequestDisplay != PullRequestDisplayMode.None || !(branch is IPullRequest))
                    yield return branch;
            }
        }

        public async IAsyncEnumerable<IBranch> FetchRemovedBranches()
        {
            if (_branchProvider == null)
                yield break;

            await foreach (var branch in _branchProvider.RemovedBranches())
            {
                yield return branch;
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

        public Task UpdateBuildBranches(IEnumerable<IBuild> builds, IEnumerable<IBranch> branches)
        {
            var branchList = branches.ToList();
            var enrichedBuilds = builds.OfType<EnrichedBuild>().ToList();

            foreach (var build in enrichedBuilds)
            {
                var branch = branchList.FirstOrDefault(b => b.FullName == build.BranchName)
                             ?? branchList.FirstOrDefault(b => b.FullName == build.Branch?.FullName)
                             ?? new NullBranch();
                build.Branch = branch;

                if (branch is IPullRequest pr)
                    build.BranchName = ExtractBranchName(pr);

                build.Links.UpdateWith(branch);
            }

            return Task.CompletedTask;
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

        private readonly IBranchProvider? _branchProvider;
        private readonly IConfiguration _applicationConfig;
        private readonly List<IBuildProvider> _buildProviders;
        private readonly ListBuildFilter _buildFilter;

        private class NullBranch : IBranch
        {
            public NullBranch()
            {
                FullName = string.Empty;
                DisplayName = string.Empty;
            }

            public string DisplayName { get; }
            public string FullName { get; }
            public bool IsPullRequest => false;

            bool IEquatable<IBranch>.Equals(IBranch other) => false;
        }
    }
}