using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Utilities
{
    internal interface IBuildSearcher
    {
        bool Matches(IBuild build, string searchTerm);
    }

    internal class BuildSearcher : IBuildSearcher
    {
        public bool Matches(IBuild build, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return true;

            var matcher = new StringMatcher(searchTerm);

            var branchName = build.BranchName;
            var definitionName = build.Definition.Name;

            return matcher.IsMatch(branchName) || matcher.IsMatch(definitionName);
        }
    }
}