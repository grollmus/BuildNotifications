using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.PluginInterfaces.Builds.Search;
using BuildNotifications.ViewModel;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.Resources.Search
{
    internal class SearchBlockViewModel : BaseViewModel
    {
        protected SearchBlockViewModel(ISearchCriteria searchCriteria, Action<SearchSuggestionViewModel> applySuggestionAction)
        {
            SearchCriteria = searchCriteria;
            ApplySuggestionAction = applySuggestionAction;
        }

        public string Description => SearchCriteria.LocalizedDescription;

        public ObservableCollection<SearchBlockExampleViewModel> Examples { get; } = new ObservableCollection<SearchBlockExampleViewModel>();

        public bool HasSuggestions => Suggestions.Any(x => !x.IsRemoving);

        public string Keyword => SearchCriteria.LocalizedKeyword;
        public ISearchCriteria SearchCriteria { get; }
        protected Action<SearchSuggestionViewModel> ApplySuggestionAction { get; }

        public SearchSuggestionViewModel? SelectedSuggestion
        {
            get => _selectedSuggestion;
            set
            {
                _selectedSuggestion = value;
                OnPropertyChanged();
            }
        }

        public int SelectedSuggestionIndex
        {
            get => _selectedSuggestionIndex;
            set
            {
                _selectedSuggestionIndex = value;
                OnPropertyChanged();
            }
        }

        public RemoveTrackingObservableCollection<SearchSuggestionViewModel> Suggestions { get; } = new RemoveTrackingObservableCollection<SearchSuggestionViewModel>(TimeSpan.FromMilliseconds(100));

        public void ChangeSelectedSuggestionIndex(int indexChange)
        {
            var newIndex = SelectedSuggestionIndex + indexChange;
            if (newIndex < 0)
                newIndex = Suggestions.Count - 1;

            if (newIndex >= Suggestions.Count)
                newIndex = Suggestions.Count == 0 ? -1 : 0;

            SelectedSuggestionIndex = newIndex;
        }

        public void ClearSuggestions()
        {
            var hadSuggestionsBeforeUpdate = HasSuggestions;

            Suggestions.Clear();
            SelectedSuggestionIndex = 0;

            if (hadSuggestionsBeforeUpdate != HasSuggestions)
                OnPropertyChanged(nameof(HasSuggestions));
        }

        public static SearchBlockViewModel FromSearchCriteria(ISearchCriteria searchCriteria, ISearchHistory history, Action<SearchSuggestionViewModel> applySuggestionAction)
        {
            if (searchCriteria is DefaultSearchCriteria defaultSearchCriteria)
                return new DefaultSearchBlockViewModel(defaultSearchCriteria, history, applySuggestionAction);

            var searchBlockViewModel = new SearchBlockViewModel(searchCriteria, applySuggestionAction);

            foreach (var example in searchCriteria.LocalizedExamples)
            {
                searchBlockViewModel.Examples.Add(new SearchBlockExampleViewModel($" {searchCriteria.LocalizedKeyword}: ", example));
            }

            return searchBlockViewModel;
        }

        protected virtual IEnumerable<SearchSuggestionViewModel> SuggestionsForSearchTerm(string searchTerm)
            => SearchCriteria.Suggest(searchTerm).Select(s => new SearchSuggestionViewModel(s, ApplySuggestionAction));

        public void UpdateSuggestions(string currentSearchTerm)
        {
            var selectedIndexBeforeUpdate = SelectedSuggestionIndex;
            var hadSuggestionsBeforeUpdate = HasSuggestions;
            var newSuggestions = SuggestionsForSearchTerm(currentSearchTerm).ToList();

            var toRemove = Suggestions.Where(s => !newSuggestions.Any(n => n.IsSameSuggestion(s))).ToList();
            var toAdd = newSuggestions.Where(n => !Suggestions.Any(s => s.IsSameSuggestion(n))).ToList();

            foreach (var suggestionToAdd in toAdd)
            {
                Suggestions.Add(suggestionToAdd);
            }

            SortSuggestions(newSuggestions);

            foreach (var suggestionToRemove in toRemove)
            {
                Suggestions.Remove(suggestionToRemove);
            }

            if (Suggestions.Any(s => !s.IsRemoving) && (SelectedSuggestion == null || SelectedSuggestion.IsRemoving))
                SelectedSuggestion = Suggestions.First(s => !s.IsRemoving);
            else
            {
                // keep the selection on the first item, except when the user has previously selected another item (and the index is != 0)
                if (selectedIndexBeforeUpdate == 0 && SelectedSuggestionIndex != 0)
                    SelectedSuggestionIndex = 0;
            }

            if (hadSuggestionsBeforeUpdate != HasSuggestions)
                OnPropertyChanged(nameof(HasSuggestions));
        }

        private void SortSuggestions(IList<SearchSuggestionViewModel> newSuggestions)
        {
            Suggestions.Sort(s =>
            {
                var index = newSuggestions.IndexOf(newSuggestions.FirstOrDefault(nS => nS.SuggestedText.Equals(s.SuggestedText, StringComparison.InvariantCulture)));
                if (index == -1)
                    index = Suggestions.IndexOf(s);

                return index;
            });
        }

        private SearchSuggestionViewModel? _selectedSuggestion;

        private int _selectedSuggestionIndex;
    }
}