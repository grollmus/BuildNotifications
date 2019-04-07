using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class BuildCombiner : IBuildCombiner
    {
        /// <inheritdoc />
        public IPipelineBuild Combine(IBuild build, IBuildDefinition buildDefinition, IBranch branch)
        {
            return new PipelineBuild(build, buildDefinition, branch);
        }
    }
}