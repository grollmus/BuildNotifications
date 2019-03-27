using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class Project : IProject
    {
        public Project(IBuildProvider buildProvider, IBranchProvider branchProvider)
        {
            BuildProvider = buildProvider;
            BranchProvider = branchProvider;
        }

        /// <inheritdoc />
        public IBranchProvider BranchProvider { get; }

        /// <inheritdoc />
        public IBuildProvider BuildProvider { get; }
    }
}