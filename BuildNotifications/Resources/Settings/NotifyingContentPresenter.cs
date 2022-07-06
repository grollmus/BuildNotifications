using System;
using System.Windows;
using System.Windows.Controls;

namespace BuildNotifications.Resources.Settings;

internal class NotifyingContentPresenter : ContentPresenter
{
    public object NotifyingContent
    {
        get => GetValue(NotifyingContentProperty);
        set => SetValue(NotifyingContentProperty, value);
    }

    public event EventHandler? ContentUpdated;

    private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not NotifyingContentPresenter notifyingContent)
            return;
        notifyingContent.RaiseContentChanged();
    }

    private void RaiseContentChanged()
    {
        ContentUpdated?.Invoke(this, EventArgs.Empty);
    }

    public static readonly DependencyProperty NotifyingContentProperty = DependencyProperty.Register(
        "NotifyingContent", typeof(object), typeof(NotifyingContentPresenter), new PropertyMetadata(default, PropertyChangedCallback));
}