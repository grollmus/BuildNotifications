using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class NullBranchNameExtractor : IBranchNameExtractor
    {
        public string ExtractDisplayName(string fullBranchName) => fullBranchName;
    }
}