using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.DummyServer;

internal class DummyBuildLinks : IBuildLinks
{
    public string BuildWeb { get; } = "https://ci.appveyor.com/project/TheSylence/buildnotifications";
    public string BranchWeb { get; } = null;
    public string DefinitionWeb { get; } = "https://github.com/grollmus/BuildNotifications";

    public void UpdateWith(IBranch branch)
    {
        // Dummy implementation
    }
}