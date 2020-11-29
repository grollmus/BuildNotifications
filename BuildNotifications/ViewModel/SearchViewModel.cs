using System;
using System.Windows.Threading;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree.Search;

namespace BuildNotifications.ViewModel
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel(IPipeline pipeline, ISearchEngine searchEngine, ISearchHistory searchHistory)
        {
            _pipeline = pipeline;
            SearchEngine = searchEngine;
            SearchHistory = searchHistory;
            SearchEngine.SearchParsed += SearchEngineOnSearchParsed;

            _searchTimer = new DispatcherTimer(DispatcherPriority.Input)
            {
                Interval = TimeSpan.FromMilliseconds(400)
            };
            _searchTimer.Tick += SearchTimerOnTick;
        }

        public string LastSearchedTerm
        {
            get => _lastSearchedTerm;
            set
            {
                _lastSearchedTerm = value;
                OnPropertyChanged();
            }
        }

        public ISearchEngine SearchEngine { get; }

        public ISearchHistory SearchHistory { get; }

        public bool TextIsEmpty => LastSearchedTerm.Length == 0;

        private void SearchEngineOnSearchParsed(object? sender, SearchEngineEventArgs e)
        {
            _lastParsedSearch = e.CreatedSearch;
            LastSearchedTerm = e.ParsedText;
            OnPropertyChanged(nameof(TextIsEmpty));
            StartSearchTask();
        }

        private void SearchTimerOnTick(object? sender, EventArgs e)
        {
            _pipeline.Search(_lastParsedSearch);

            _searchTimer.Stop();
        }

        private void StartSearchTask()
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private readonly IPipeline _pipeline;
        private readonly DispatcherTimer _searchTimer;
        private ISpecificSearch _lastParsedSearch = new EmptySearch();

        private string _lastSearchedTerm = string.Empty;
    }
}