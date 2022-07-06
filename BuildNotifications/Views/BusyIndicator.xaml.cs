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

namespace BuildNotifications.Views;

public partial class BusyIndicator : INotifyPropertyChanged, IDisposable
{
    public BusyIndicator()
    {
        InitializeComponent();
        DummyItems = new ObservableCollection<DummyItem>
        {
            new(),
            new()
        };
        _tokenSource = new CancellationTokenSource();
    }

    public ObservableCollection<DummyItem> DummyItems { get; }

    public bool IsBusy
    {
        get => (bool)GetValue(IsBusyProperty);
        set => SetValue(IsBusyProperty, value);
    }

    public bool ShowLoadingText
    {
        get => _showLoadingText;
        set
        {
            _showLoadingText = value;
            OnPropertyChanged();
        }
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.Property.Equals(IsBusyProperty) && d is BusyIndicator busyIndicator)
        {
            if ((bool)e.NewValue)
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
                var firstItem = DummyItems.FirstOrDefault(x => !x.IsRemoving);

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

    public void Dispose()
    {
        _removeAndAddingTask?.Dispose();
        _tokenSource.Dispose();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private bool _showLoadingText;

    private Task? _removeAndAddingTask;
    private CancellationTokenSource _tokenSource;

    public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register(
        "IsBusy", typeof(bool), typeof(BusyIndicator), new PropertyMetadata(default(bool), PropertyChangedCallback));

    public class DummyItem : BaseViewModel
    {
    }
}