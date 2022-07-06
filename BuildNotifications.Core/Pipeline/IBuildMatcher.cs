using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline;

internal interface IBuildMatcher
{
    bool IsMatch(IBaseBuild build);
}