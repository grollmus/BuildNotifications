using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class DummyBuildLinks : IBuildLinks
    {
        public string BuildWeb { get; } = "https://ci.appveyor.com/project/TheSylence/buildnotifications";
        public string BranchWeb { get; } = null;
        public string DefinitionWeb { get; } = "https://github.com/grollmus/BuildNotifications";
    }
}