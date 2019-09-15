using System;
using System.Windows.Threading;
using BuildNotifications.Core.Pipeline;

namespace BuildNotifications.ViewModel
{
    public class SearchViewModel : BaseViewModel
    {
        public SearchViewModel(IPipeline pipeline)
        {
            _pipeline = pipeline;
            _searchTerm = string.Empty;

            _searchTimer = new DispatcherTimer(DispatcherPriority.Input)
            {
                Interval = TimeSpan.FromMilliseconds(250)
            };
            _searchTimer.Tick += SearchTimerOnTick;
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

                StartSearchTask();
            }
        }

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
        private string _searchTerm;
    }
}