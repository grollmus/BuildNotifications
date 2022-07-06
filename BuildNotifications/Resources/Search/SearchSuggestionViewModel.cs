using System;
using System.Windows.Input;
using BuildNotifications.PluginInterfaces.Builds.Search;
using BuildNotifications.ViewModel;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.Resources.Search;

internal class SearchSuggestionViewModel : BaseViewModel
{
    public SearchSuggestionViewModel(ISearchCriteriaSuggestion searchCriteriaSuggestion, Action<SearchSuggestionViewModel> selectSuggestionAction)
    {
        IsFromHistory = false;
        IsKeyword = searchCriteriaSuggestion.IsKeyword;
        SuggestedText = searchCriteriaSuggestion.Suggestion;
        DeleteSuggestionCommand = new DelegateCommand(() =>
        {
            // do nothing for suggestions not originating on history
        });
        SelectSuggestionCommand = new DelegateCommand(() => selectSuggestionAction(this));
    }

    public SearchSuggestionViewModel(string term, Action<string> deleteSuggestionAction, Action<SearchSuggestionViewModel> selectSuggestionAction)
    {
        IsKeyword = false;
        IsFromHistory = true;
        SuggestedText = term;
        DeleteSuggestionCommand = new DelegateCommand(() => deleteSuggestionAction(term));
        SelectSuggestionCommand = new DelegateCommand(() => selectSuggestionAction(this));
    }

    public ICommand DeleteSuggestionCommand { get; }

    public bool IsFromHistory { get; }

    public bool IsKeyword { get; }

    public ICommand SelectSuggestionCommand { get; }

    public string SuggestedText { get; }

    public bool IsSameSuggestion(SearchSuggestionViewModel otherViewModel) => otherViewModel.SuggestedText.Equals(SuggestedText, StringComparison.InvariantCulture);
}