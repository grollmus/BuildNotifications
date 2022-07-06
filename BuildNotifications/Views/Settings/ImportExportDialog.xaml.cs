using System;
using System.Windows;
using BuildNotifications.ViewModel;

namespace BuildNotifications.Views.Settings;

public partial class ImportExportDialog
{
    public ImportExportDialog()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;
    }

    private void OnCloseRequested(object? sender, EventArgs e)
    {
        Close();
    }

    private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (e.OldValue is IRequestClose oldRequest)
            oldRequest.CloseRequested -= OnCloseRequested;

        if (e.NewValue is IRequestClose newRequest)
            newRequest.CloseRequested += OnCloseRequested;
    }
}