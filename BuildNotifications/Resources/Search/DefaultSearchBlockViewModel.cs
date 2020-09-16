using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Search;

namespace BuildNotifications.Resources.Search
{
    internal class DefaultSearchBlockViewModel : SearchBlockViewModel
    {
        private readonly ISearchHistory _history;
        private string _lastUsedSearchTerm = string.Empty;

        protected override IEnumerable<SearchSuggestionViewModel> SuggestionsForSearchTerm(string searchTerm)
        {
            _lastUsedSearchTerm = searchTerm;

            if (string.IsNullOrEmpty(searchTerm))
                return HistoryAsSuggestions().Concat(base.SuggestionsForSearchTerm(searchTerm));

            return base.SuggestionsForSearchTerm(searchTerm);
        }

        private IEnumerable<SearchSuggestionViewModel> HistoryAsSuggestions()
        {
            const int suggestedValuesFromHistory = 5;
            return _history.SearchedTerms().Select(t => new SearchSuggestionViewModel(t, DeleteSuggestion, ApplySuggestionAction)).Take(suggestedValuesFromHistory);
        }

        private void DeleteSuggestion(string termToDelete)
        {
            _history.RemoveEntry(termToDelete);
            UpdateSuggestions(_lastUsedSearchTerm);
        }

        public DefaultSearchBlockViewModel(DefaultSearchCriteria defaultSearchCriteria, ISearchHistory history, Action<SearchSuggestionViewModel> applySuggestionAction)
            : base(defaultSearchCriteria, applySuggestionAction)
        {
            _history = history;
            foreach (var (keyword, exampleTerm) in defaultSearchCriteria.ExamplesFromEachSubCriteria())
            {
                Examples.Add(new SearchBlockExampleViewModel($" {keyword}: ", exampleTerm));
            }
        }
    }
}