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
            _searchEngine = searchEngine;
            _searchTerm = string.Empty;

            _searchTimer = new DispatcherTimer(DispatcherPriority.Input)
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            _searchTimer.Tick += SearchTimerOnTick;
        }

        public ISearchEngine SearchEngine
        {
            get => _searchEngine;
            set
            {
                _searchEngine = value;
                OnPropertyChanged();
            }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set
            {
                if (_searchTerm == value)
                    return;

                _searchTerm = value;
                OnPropertyChanged(nameof(SearchTerm));
                OnPropertyChanged(nameof(TextIsEmpty));

                StartSearchTask();
            }
        }

        public bool TextIsEmpty => SearchTerm.Length == 0;

        private void SearchTimerOnTick(object? sender, EventArgs e)
        {
            _pipeline.Search(SearchTerm);

            _searchTimer.Stop();
        }

        private void StartSearchTask()
        {
            _searchTimer.Stop();
            _searchTimer.Start();
        }

        private readonly IPipeline _pipeline;
        private readonly DispatcherTimer _searchTimer;

        private ISearchEngine _searchEngine;
        private string _searchTerm;
    }
}