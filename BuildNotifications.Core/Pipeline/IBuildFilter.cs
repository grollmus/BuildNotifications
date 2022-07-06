using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline;

internal interface IBuildFilter
{
    bool IsAllowed(IBaseBuild build);
}