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

            var term = searchTerm.ToLowerInvariant();
            var branchName = build.BranchName.ToLowerInvariant();
            var definitionName = build.Definition.Name.ToLowerInvariant();

            return branchName.Contains(term) || definitionName.Contains(term);
        }
    }
}