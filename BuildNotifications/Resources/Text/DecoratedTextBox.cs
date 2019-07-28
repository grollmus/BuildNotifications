using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.Resources.Text
{
    internal class DecoratedTextBox : TextBox
    {
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(IconType), typeof(DecoratedTextBox), new PropertyMetadata(default(IconType)));

        public IconType Icon
        {
            get => (IconType) GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(DecoratedTextBox), new PropertyMetadata(default(string)));

        private ScrollViewer _scrollViewer;
        private Border _overlay;

        public string Label
        {
            get => (string) GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        public DecoratedTextBox()
        {
            GotFocus += OnGotFocus;
            LostFocus += OnLostFocus;
            PreviewMouseDown += OnPreviewMouseDown;
            PreviewKeyDown += OnKeyDown;
            KeyUp += OnKeyDown;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            ResetScrollViewerMargin();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            CheckForScrollViewerCollision();
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

        private void ResetScrollViewerMargin()
        {
            _scrollViewer.Margin = new Thickness(0);
            _scrollViewer.MaxWidth = double.MaxValue;
        }

        private void SetScrollViewerMarginToAvoidLabelOrIcon()
        {
            var availableHeight = Math.Min(double.MaxValue, MaxHeight);

            var neededMargin = RenderSize(_overlay).Height;
            if (RenderSize(_scrollViewer).Height + neededMargin > availableHeight)
            {
                var sizeOfOverlay = RenderSize(_overlay).Width;
                var availableSize = RenderSize(this).Width;
                var sizeWithoutOverlay = availableSize - sizeOfOverlay;
                _scrollViewer.MaxWidth = sizeWithoutOverlay + 1;
                return;
            }

            _scrollViewer.Margin = new Thickness(0, 0, 0, neededMargin);
            _scrollViewer.MaxWidth = double.MaxValue;
        }

        private bool ScrollViewerReachesIntoLabelOrIcon()
        {
            var textSize = RenderSize(_scrollViewer).Width;
            var sizeOfOverlay = RenderSize(_overlay).Width;
            var availableSize = RenderSize(this).Width;

            return textSize + sizeOfOverlay > availableSize;
        }

        private Size RenderSize(FrameworkElement element)
        {
            if (element.Visibility == Visibility.Collapsed)
                return new Size(0, 0);

            return element.RenderSize;
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

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            CheckForScrollViewerCollision();
            if (string.IsNullOrWhiteSpace(SelectedText))
                SelectAll();
        }

        public override void OnApplyTemplate()
        {
            _scrollViewer = GetTemplateChild("PART_ContentHost") as ScrollViewer;
            _overlay = GetTemplateChild("Overlay") as Border;
            base.OnApplyTemplate();
        }
    }
}