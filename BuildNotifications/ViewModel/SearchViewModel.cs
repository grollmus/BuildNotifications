using System;
using System.Windows.Threading;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree.Search;

namespace BuildNotifications.ViewModel
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel(IPipeline pipeline, ISearchEngine searchEngine)
        {
            _pipeline = pipeline;
            SearchEngine = searchEngine;
            SearchEngine.SearchParsed += SearchEngineOnSearchParsed;

            _searchTimer = new DispatcherTimer(DispatcherPriority.Input)
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            _searchTimer.Tick += SearchTimerOnTick;
        }

        private void SearchEngineOnSearchParsed(object? sender, SearchEngineEventArgs e)
        {
            _lastParsedSearch = e.CreatedSearch;
            _lastParsedSearchTerm = e.ParsedText;
            OnPropertyChanged(nameof(TextIsEmpty));
            StartSearchTask();
        }

        public ISearchEngine SearchEngine { get; }

        public bool TextIsEmpty => _lastParsedSearchTerm.Length == 0;

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

        private string _lastParsedSearchTerm = string.Empty;
    }
}