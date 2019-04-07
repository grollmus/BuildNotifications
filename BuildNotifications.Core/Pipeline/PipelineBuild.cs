using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class PipelineBuild : IPipelineBuild
    {
        public PipelineBuild(IBuild build, IBuildDefinition buildDefinition, IBranch branch)
        {
            _build = build;
            _buildDefinition = buildDefinition;
            _branch = branch;
        }

        private readonly IBuild _build;
        private readonly IBuildDefinition _buildDefinition;
        private readonly IBranch _branch;
    }
}