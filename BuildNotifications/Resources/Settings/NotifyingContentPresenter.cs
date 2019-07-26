using System;
using System.Windows;
using System.Windows.Controls;

namespace BuildNotifications.Resources.Settings
{
    internal class NotifyingContentPresenter : ContentPresenter
    {
        public event EventHandler ContentUpdated;

        public static readonly DependencyProperty NotifyingContentProperty = DependencyProperty.Register(
            "NotifyingContent", typeof(object), typeof(NotifyingContentPresenter), new PropertyMetadata(default(object), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is NotifyingContentPresenter notifyingContent))
                return;
            notifyingContent.RaiseContentChanged();
        }

        private void RaiseContentChanged()
        {
            ContentUpdated?.Invoke(this, EventArgs.Empty);
        }

        public object NotifyingContent
        {
            get => (object) GetValue(NotifyingContentProperty);
            set => SetValue(NotifyingContentProperty, value);
        }
    }
}
