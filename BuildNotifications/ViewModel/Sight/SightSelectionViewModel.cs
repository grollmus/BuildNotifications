using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using BuildNotifications.ViewModel.Sight.Specific;

namespace BuildNotifications.ViewModel.Sight
{
    public class SightSelectionViewModel : BaseViewModel
    {
        public ObservableCollection<BaseSightViewModel> Sights { get; } = new ObservableCollection<BaseSightViewModel>();

        public event EventHandler? SightSelectionChanged;

        public SightSelectionViewModel()
        {
            foreach (var sightViewModel in CreateSightViewModels())
            {
                Sights.Add(sightViewModel);
                sightViewModel.PropertyChanged += OnSightPropertyChanged;
            }
        }

        private void OnSightPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(BaseSightViewModel.IsEnabled))
                return;

            SightSelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        private IEnumerable<BaseSightViewModel> CreateSightViewModels()
        {
            yield return new ShowOnlyManualBuildsSightViewModel();
            yield return new HighlightMyBuildsSightViewModel();
        }
    }
}