using System;
using System.Collections.Generic;
using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class Project : IProject
    {
        public Project(IBuildProvider buildProvider, IBranchProvider branchProvider, IProjectConfiguration config)
        {
            Name = config.ProjectName;
            BuildProvider = buildProvider;
            BranchProvider = branchProvider;
            Config = config;
        }

        private IBuild Enrich(IBaseBuild build)
        {
            return new EnrichedBuild(build, Name);
        }

        public IBranchProvider BranchProvider { get; set; }

        /// <inheritdoc />
        public IBuildProvider BuildProvider { get; set; }

        /// <inheritdoc />
        public IProjectConfiguration Config { get; set; }

        /// <inheritdoc />
        public string Name { get; set; }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBuild> FetchAllBuilds()
        {
            await foreach (var build in BuildProvider.FetchAllBuilds())
            {
                yield return Enrich(build);
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<IBuild> FetchBuildsChangedSince(DateTime lastUpdate)
        {
            await foreach (var build in BuildProvider.FetchBuildsChangedSince(lastUpdate))
            {
                yield return Enrich(build);
            }
        }

        public async IAsyncEnumerable<IBuild> FetchRemovedBuilds()
        {
            await foreach (var build in BuildProvider.RemovedBuilds())
            {
                yield return Enrich(build);
            }
        }

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