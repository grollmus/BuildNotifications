using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    internal class SearchCriteriaSuggestion : ISearchCriteriaSuggestion
    {
        public SearchCriteriaSuggestion(string suggestion)
        {
            Suggestion = suggestion;
        }

        public string Suggestion { get; }
    }
}