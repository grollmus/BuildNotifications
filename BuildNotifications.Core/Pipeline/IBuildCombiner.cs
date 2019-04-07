using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    /// <summary>
    /// Combines build, build definition and branch information from different
    /// sources into one single item that is used in the pipeline.
    /// </summary>
    internal interface IBuildCombiner
    {
        IPipelineBuild Combine(IBuild build, IBuildDefinition buildDefinition, IBranch branch);
    }
}