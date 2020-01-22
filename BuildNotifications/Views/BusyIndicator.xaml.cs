using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using BuildNotifications.ViewModel;
using JetBrains.Annotations;

namespace BuildNotifications.Views
{
    public partial class BusyIndicator : INotifyPropertyChanged
    {
        public BusyIndicator()
        {
            InitializeComponent();
            DummyItems = new ObservableCollection<DummyItem>
            {
                new DummyItem(),
                new DummyItem(),
            };
            _tokenSource = new CancellationTokenSource();
        }

        public ObservableCollection<DummyItem> DummyItems { get; set; }

        public bool IsBusy
        {
            get => (bool) GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        private bool _showLoadingText;

        public bool ShowLoadingText
        {
            get => _showLoadingText;
            set
            {
                _showLoadingText = value;
                OnPropertyChanged();
            }
        }

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Equals(IsBusyProperty) && d is BusyIndicator busyIndicator)
            {
                if ((bool) e.NewValue)
                    busyIndicator.StartTask();
                else
                    busyIndicator.StopTask();
            }
        }

        private void StartTask()
        {
            if (_removeAndAddingTask != null)
                return;

            _removeAndAddingTask = Task.Run(async () =>
            {
                while (!_tokenSource.IsCancellationRequested)
                {
                    ShowLoadingText = App.GlobalTweenHandler.TimeModifier > 2;
                    var firstItem = DummyItems.FirstOrDefault(x => !(x?.IsRemoving ?? false));

                    if (firstItem != null)
                        firstItem.IsRemoving = true;

                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(Math.Max(0.5, 1.0 / App.GlobalTweenHandler.TimeModifier)), _tokenSource.Token);
                    }
                    catch (Exception)
                    {
                        break;
                    }

                    if (!_tokenSource.IsCancellationRequested && firstItem != null)
                    {
                        Application.Current?.Dispatcher?.Invoke(() =>
                        {
                            DummyItems.Remove(firstItem);
                            DummyItems.Add(new DummyItem());
                        });
                    }
                }

                ShowLoadingText = false;
            }, _tokenSource.Token);
        }

        private void StopTask()
        {
            if (_removeAndAddingTask == null || _removeAndAddingTask.Status == TaskStatus.WaitingForActivation)
                return;

            _tokenSource.Cancel();
            _removeAndAddingTask.Wait();
            _removeAndAddingTask = null;
            _tokenSource = new CancellationTokenSource();
            ShowLoadingText = false;
        }

        private Task? _removeAndAddingTask;
        private CancellationTokenSource _tokenSource;

        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
            "IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(default(bool), PropertyChangedCallback));

        public class DummyItem : BaseViewModel
        {
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}