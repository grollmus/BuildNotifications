using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    public interface ISearchBlock
    {
        ISearchCriteria SearchCriteria { get; }

        string SearchedText { get; }

        bool IsBuildIncluded(IBuild build, string input);
    }
}