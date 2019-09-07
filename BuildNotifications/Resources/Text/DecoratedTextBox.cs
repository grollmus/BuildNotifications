using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.Resources.Text
{
    internal class DecoratedTextBox : TextBox
    {
        public DecoratedTextBox()
        {
            GotFocus += OnGotFocus;
            LostFocus += OnLostFocus;
            PreviewMouseDown += OnPreviewMouseDown;
            PreviewKeyDown += OnKeyDown;
            KeyUp += OnKeyDown;

            // use dummy instances for a quick and easy way to avoid null reference exceptions
            _scrollViewer = new ScrollViewer();
            _overlay = new Border();
        }

        public IconType Icon
        {
            get => (IconType) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Label
        {
            get => (string) GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public override void OnApplyTemplate()
        {
            _scrollViewer = GetTemplateChild("PART_ContentHost") as ScrollViewer ?? new ScrollViewer();
            _overlay = GetTemplateChild("Overlay") as Border ?? new Border();
            base.OnApplyTemplate();
        }

        private void CheckForScrollViewerCollision()
        {
            if (_scrollViewer == null || _overlay == null)
                return;

            if (ScrollViewerReachesIntoLabelOrIcon())
                SetScrollViewerMarginToAvoidLabelOrIcon();
            else
                ResetScrollViewerMargin();
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            CheckForScrollViewerCollision();
            if (string.IsNullOrWhiteSpace(SelectedText))
                SelectAll();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            CheckForScrollViewerCollision();
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            ResetScrollViewerMargin();
        }

        private void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsFocused)
                return;

            if (!string.IsNullOrWhiteSpace(SelectedText))
                return;

            Focus();
            SelectAll();
            e.Handled = true;
        }

        private Size RenderSizeOfElement(FrameworkElement element)
        {
            if (element.Visibility == Visibility.Collapsed)
                return new Size(0, 0);

            return element.RenderSize;
        }

        private void ResetScrollViewerMargin()
        {
            _scrollViewer.Margin = new Thickness(0);
            _scrollViewer.MaxWidth = double.MaxValue;
        }

        private bool ScrollViewerReachesIntoLabelOrIcon()
        {
            var textSize = RenderSizeOfElement(_scrollViewer).Width;
            var sizeOfOverlay = RenderSizeOfElement(_overlay).Width;
            var availableSize = RenderSizeOfElement(this).Width;

            return textSize + sizeOfOverlay > availableSize;
        }

        private void SetScrollViewerMarginToAvoidLabelOrIcon()
        {
            var availableHeight = Math.Min(double.MaxValue, MaxHeight);

            var neededMargin = RenderSizeOfElement(_overlay).Height;
            if (RenderSizeOfElement(_scrollViewer).Height + neededMargin > availableHeight)
            {
                var sizeOfOverlay = RenderSizeOfElement(_overlay).Width;
                var availableSize = RenderSizeOfElement(this).Width;
                var sizeWithoutOverlay = availableSize - sizeOfOverlay;
                _scrollViewer.MaxWidth = sizeWithoutOverlay + 1;
                return;
            }

            _scrollViewer.Margin = new Thickness(0, 0, 0, neededMargin);
            _scrollViewer.MaxWidth = double.MaxValue;
        }

        private ScrollViewer _scrollViewer;
        private Border _overlay;

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(IconType), typeof(DecoratedTextBox), new PropertyMetadata(default(IconType)));

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(DecoratedTextBox), new PropertyMetadata(default(string)));
    }
}