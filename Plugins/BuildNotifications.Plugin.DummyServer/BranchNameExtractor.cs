using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Plugin.DummyServer
{
    internal class BranchNameExtractor : IBranchNameExtractor
    {
        public string ExtractDisplayName(string fullBranchName) => fullBranchName;
    }
}