using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Anotar.NLog;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Utilities;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class Project : IProject
    {
        private readonly IBranchNameExtractor _branchNameExtractor;

        public Project(IEnumerable<IBuildProvider> buildProviders, IEnumerable<IBranchProvider> branchProviders,
            IProjectConfiguration config, IBranchNameExtractor branchNameExtractor)
        {
            _branchNameExtractor = branchNameExtractor;
            Name = config.ProjectName;
            _buildProviders = buildProviders.ToList();
            _branchProviders = branchProviders.ToList();
            Config = config;

            _buildFilter = new ListBuildFilter(config);
        }

        public Project(IBuildProvider buildProvider, IBranchProvider branchProvider, IProjectConfiguration config, IBranchNameExtractor branchNameExtractor)
            : this(buildProvider.Yield(), branchProvider.Yield(), config, branchNameExtractor)
        {
        }

        private IBuild Enrich(IBaseBuild build, IBuildProvider buildProvider)
        {
            return new EnrichedBuild(build, Name, buildProvider);
        }

        private bool IsAllowed(IBaseBuild build)
        {
            return _buildFilter.IsAllowed(build);
        }

        public IProjectConfiguration Config { get; set; }

        public string Name { get; set; }

        public async IAsyncEnumerable<IBuild> FetchAllBuilds()
        {
            _buildFilter.InitializeStringMatcher();

            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var build in buildProvider.FetchAllBuilds())
                {
                    if (IsAllowed(build))
                        yield return Enrich(build, buildProvider);
                    else
                        LogTo.Debug($"Build {build.Definition.Name}.{build.Id} on {build.BranchName} fetched but was filtered");
                }
            }
        }

        public async IAsyncEnumerable<IBuild> FetchBuildsChangedSince(DateTime lastUpdate)
        {
            _buildFilter.InitializeStringMatcher();

            foreach (var buildProvider in _buildProviders)
            {
                await foreach (var build in buildProvider.FetchBuildsChangedSince(lastUpdate))
                {
                    if (IsAllowed(build))
                        yield return Enrich(build, buildProvider);
                    else
                        LogTo.Debug($"Build {build.Definition.Name}.{build.Id} on {build.BranchName} fetched but was filtered");
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

        public Task UpdateBuildBranches(IEnumerable<IBuild> builds, IEnumerable<IBranch> branches)
        {
            var branchList = branches.ToList();
            var enrichedBuilds = builds.OfType<EnrichedBuild>().ToList();

            foreach (var build in enrichedBuilds)
            {
                var branch = branchList.FirstOrDefault(b => b.Name == build.BranchName);
                if (branch == null)
                {
                    LogTo.Debug($"Did not find branch with name '{build.BranchName}' in branches for project");
                    continue;
                }

                build.Branch = branch;

                if (branch is IPullRequest pr)
                    build.BranchName = ExtractBranchName(pr);
            }

            return Task.CompletedTask;
        }

        private string ExtractBranchName(IPullRequest pr)
        {
            switch (Config.PullRequestDisplay)
            {
                case PullRequestDisplayMode.Name:
                    return pr.Description;
                case PullRequestDisplayMode.Path:
                    var sourceName = _branchNameExtractor.ExtractDisplayName(pr.SourceBranch);
                    var targetName = _branchNameExtractor.ExtractDisplayName(pr.TargetBranch);
                    return $"{sourceName} into {targetName}";

                default:
                    return $"PR {pr.Id}";
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
        private readonly ListBuildFilter _buildFilter;
    }
}