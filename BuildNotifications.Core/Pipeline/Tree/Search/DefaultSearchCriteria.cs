using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    /// <summary>
    /// Searches through multiple given criteria. If one matches, the build is included.
    /// </summary>
    public class DefaultSearchCriteria : ISearchCriteria
    {
        private readonly IReadOnlyList<ISearchCriteria> _includedCriterions;

        private readonly IReadOnlyList<ISearchCriteriaSuggestion> _criterionsAsSuggestions;

        public DefaultSearchCriteria(IEnumerable<ISearchCriteria> includedCriterions)
        {
            _includedCriterions = includedCriterions.ToList();
            _criterionsAsSuggestions = _includedCriterions.Select(s => new SearchCriteriaAsSuggestion(s)).ToList();
        }

        public string LocalizedKeyword => "";

        public string LocalizedDescription => StringLocalizer.SearchDefaultDescription;

        public IEnumerable<ISearchCriteriaSuggestion> Suggest(string input)
        {
            const int suggestionsToTakeFromEachCriteria = 2;

            foreach (var searchCriteriaSuggestion in _criterionsAsSuggestions)
            {
                if (string.IsNullOrEmpty(input) || searchCriteriaSuggestion.Suggestion.StartsWith(input, StringComparison.InvariantCultureIgnoreCase))
                    yield return searchCriteriaSuggestion;
            }

            foreach (var searchCriteria in _includedCriterions)
            {
                foreach (var suggestion in searchCriteria.Suggest(input).Take(suggestionsToTakeFromEachCriteria))
                {
                    yield return suggestion;
                }
            }
        }

        public bool IsBuildIncluded(IBuild build, string input) => string.IsNullOrWhiteSpace(input) || _includedCriterions.Any(c => c.IsBuildIncluded(build, input));

        public IEnumerable<string> LocalizedExamples => ExamplesFromEachSubCriteria().Select(t => t.exampleTerm);

        public IEnumerable<(string keyword, string exampleTerm)> ExamplesFromEachSubCriteria()
        {
            const int suggestionsToTakeFromEachCriteria = 1;

            foreach (var searchCriteria in _includedCriterions)
            {
                foreach (var example in searchCriteria.LocalizedExamples.Take(suggestionsToTakeFromEachCriteria))
                {
                    yield return (searchCriteria.LocalizedKeyword, example);
                }
            }
        }
    }

    public class SearchCriteriaAsSuggestion : ISearchCriteriaSuggestion
    {
        public SearchCriteriaAsSuggestion(ISearchCriteria searchCriteria)
        {
            Suggestion = searchCriteria.LocalizedKeyword + SearchEngine.KeywordSeparator;
        }

        public string Suggestion { get; }
    }
}