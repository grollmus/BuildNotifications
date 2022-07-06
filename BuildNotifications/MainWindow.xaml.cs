using System;
using System.ComponentModel;
using System.Windows;
using BuildNotifications.ViewModel;
using NLog.Fluent;

namespace BuildNotifications;

public partial class MainWindow : IViewProvider
{
    public MainWindow()
    {
        DataContext = new MainViewModel(this);
        InitializeComponent();
        Visibility = App.StartMinimized ? Visibility.Hidden : Visibility.Visible;
        Closing += OnClosing;
    }

    private MainViewModel ViewModel => (MainViewModel)DataContext;

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        ViewModel.RestoreWindowStateFor(this);
    }

    private void OnClosing(object? sender, CancelEventArgs e)
    {
        ViewModel.SaveWindowStateOf(this);
        Log.Info().Message("Hiding window.").Write();
        Visibility = Visibility.Collapsed;
        e.Cancel = true;
    }

    public Window View => this;
}