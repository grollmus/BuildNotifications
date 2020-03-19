using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    internal class SearchBlock : ISearchBlock
    {
        public ISearchCriteria SearchCriteria { get; }
        public string SearchedText { get; }

        public SearchBlock(ISearchCriteria searchCriteria, string searchedText)
        {
            SearchCriteria = searchCriteria;
            SearchedText = searchedText;
        }

        public bool IsBuildIncluded(IBuild build) => SearchCriteria.IsBuildIncluded(build, SearchedText);
    }
}