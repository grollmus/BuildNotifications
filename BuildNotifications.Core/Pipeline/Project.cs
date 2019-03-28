using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class Project : IProject
    {
        public Project(IBuildProvider buildProvider, IBranchProvider branchProvider, IProjectConfiguration config)
        {
            BuildProvider = buildProvider;
            BranchProvider = branchProvider;
            Config = config;
        }

        /// <inheritdoc />
        public IBranchProvider BranchProvider { get; }

        /// <inheritdoc />
        public IBuildProvider BuildProvider { get; }

        /// <inheritdoc />
        public IProjectConfiguration Config { get; }
    }
}