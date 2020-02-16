using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class BranchNameExtractor : IBranchNameExtractor
    {
        public string ExtractDisplayName(string fullBranchName) => fullBranchName;
    }
}