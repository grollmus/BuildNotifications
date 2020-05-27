using System;
using System.Windows.Input;
using BuildNotifications.PluginInterfaces.Builds.Search;
using BuildNotifications.ViewModel;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.Resources.Search
{
    internal class SearchSuggestionViewModel : BaseViewModel
    {
        public SearchSuggestionViewModel(ISearchCriteriaSuggestion searchCriteriaSuggestion)
        {
            IsFromHistory = false;
            IsKeyword = searchCriteriaSuggestion.IsKeyword;
            SuggestedText = searchCriteriaSuggestion.Suggestion;
            DeleteSuggestionCommand = new DelegateCommand(() =>
            {
                // do nothing for suggestions not originating on history
            });
        }

        public SearchSuggestionViewModel(string term, Action<string> deleteSuggestionAction)
        {
            IsKeyword = false;
            IsFromHistory = true;
            SuggestedText = term;
            DeleteSuggestionCommand = new DelegateCommand(() => deleteSuggestionAction(term));
        }

        public ICommand DeleteSuggestionCommand { get; }

        public bool IsFromHistory { get; }

        public bool IsKeyword { get; }

        public string SuggestedText { get; }

        public bool IsSameSuggestion(SearchSuggestionViewModel otherViewModel)
        {
            return otherViewModel.SuggestedText.Equals(SuggestedText, StringComparison.InvariantCulture);
        }
    }
}