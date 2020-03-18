using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.Resources.Search
{
    internal class SearchTextBox : RichTextBox
    {
        public SearchTextBox()
        {
            GotFocus += OnGotFocus;
            LostFocus += OnLostFocus;
            PreviewMouseDown += OnPreviewMouseDown;
            PreviewKeyDown += OnKeyDown;
            KeyUp += OnKeyDown;

            // use dummy instances for a quick and easy way to avoid null reference exceptions
            _scrollViewer = new ScrollViewer();
            _overlay = new Border();

            TextChanged += OnTextChanged;
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

        public Brush SearchCriteriaBackground
        {
            get => (Brush) GetValue(SearchCriteriaBackgroundProperty);
            set => SetValue(SearchCriteriaBackgroundProperty, value);
        }

        public Brush SearchCriteriaForeground
        {
            get => (Brush) GetValue(SearchCriteriaForegroundProperty);
            set => SetValue(SearchCriteriaForegroundProperty, value);
        }

        public ISearchEngine? SearchEngine
        {
            get => (ISearchEngine) GetValue(SearchEngineProperty);
            set => SetValue(SearchEngineProperty, value);
        }

        public Brush SearchTermBackground
        {
            get => (Brush) GetValue(SearchTermBackgroundProperty);
            set => SetValue(SearchTermBackgroundProperty, value);
        }

        public Brush SearchTermForeground
        {
            get => (Brush) GetValue(SearchTermForegroundProperty);
            set => SetValue(SearchTermForegroundProperty, value);
        }

        public override void OnApplyTemplate()
        {
            _scrollViewer = GetTemplateChild("PART_ContentHost") as ScrollViewer ?? new ScrollViewer();
            _overlay = GetTemplateChild("Overlay") as Border ?? new Border();

            base.OnApplyTemplate();
        }

        private void CheckForScrollViewerCollision()
        {
            if (ScrollViewerReachesIntoLabelOrIcon())
                SetScrollViewerMarginToAvoidLabelOrIcon();
            else
                ResetScrollViewerMargin();
        }

        private string CurrentText()
        {
            var blocks = Document.Blocks;
            var paragraph = blocks.OfType<Paragraph>().FirstOrDefault();

            if (paragraph == null)
                return string.Empty;

            var sb = new StringBuilder();

            foreach (var inline in paragraph.Inlines.OfType<Run>())
            {
                sb.Append(inline.Text);
            }

            return sb.ToString();
        }

        private Run DefaultRun(string text)
        {
            var run = new Run(text);
            return run;
        }

        private void DisplaySearchBlocks(IReadOnlyList<ISearchBlock> parsedBlocks)
        {
            var blocks = Document.Blocks;
            var paragraph = blocks.OfType<Paragraph>().FirstOrDefault();
            if (paragraph == null)
            {
                paragraph = new Paragraph();
                Document.Blocks.Add(paragraph);
            }

            var inlines = paragraph.Inlines;
            inlines.Clear();

            foreach (var parsedBlock in parsedBlocks)
            {
                if (string.IsNullOrEmpty(parsedBlock.SearchCriteria.LocalizedKeyword))
                {
                    if (!string.IsNullOrEmpty(parsedBlock.SearchedText))
                        inlines.Add(DefaultRun(parsedBlock.SearchedText));
                }
                else
                {
                    inlines.Add(SearchCriteriaRun($"{parsedBlock.SearchCriteria.LocalizedKeyword}:"));
                    if (!string.IsNullOrEmpty(parsedBlock.SearchedText))
                        inlines.Add(SearchTermRun(parsedBlock.SearchedText));
                }
            }
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            CheckForScrollViewerCollision();
            if (string.IsNullOrWhiteSpace(Selection.ToString()))
                SelectAll();

            _overlayWidth = RenderSizeOfElement(_overlay).Width;
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

            if (!string.IsNullOrWhiteSpace(Selection.ToString()))
                return;

            Focus();
            SelectAll();
            e.Handled = true;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchEngine == null)
                return;

            var currentText = CurrentText();
            var parsedBlocks = SearchEngine.Parse(currentText);

            TextChanged -= OnTextChanged;
            var storedSelection = StoreSelection();

            DisplaySearchBlocks(parsedBlocks.Blocks);

            RestoreSelection(storedSelection);
            TextChanged += OnTextChanged;
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

        private void RestoreSelection((int startOffset, int lengthOffset, int amountOfInlines) storedSelection)
        {
            var inlineCountNow = Document.Blocks.OfType<Paragraph>().FirstOrDefault()?.Inlines.Count ?? 0;
            var additionalCountForInlines = (inlineCountNow - storedSelection.amountOfInlines) * 2;

            var start = Document.ContentStart.GetPositionAtOffset(storedSelection.startOffset + additionalCountForInlines);
            var end = start?.GetPositionAtOffset(storedSelection.lengthOffset);

            if (end == null)
                return;

            Selection.Select(start, end);

            // bring selection into view
            var characterRect = end.GetCharacterRect(LogicalDirection.Forward);
            ScrollToHorizontalOffset(HorizontalOffset + characterRect.Left - ActualWidth + _overlayWidth + 2d); // 2 for caret width
        }

        private bool ScrollViewerReachesIntoLabelOrIcon()
        {
            var textSize = RenderSizeOfElement(_scrollViewer).Width;
            var sizeOfOverlay = RenderSizeOfElement(_overlay).Width;
            var availableSize = RenderSizeOfElement(this).Width;

            return textSize + sizeOfOverlay > availableSize;
        }

        private Run SearchCriteriaRun(string text)
        {
            var run = new Run(text)
            {
                Background = SearchCriteriaBackground,
                Foreground = SearchCriteriaForeground,
                FontWeight = FontWeights.Bold
            };

            return run;
        }

        private Run SearchTermRun(string text)
        {
            var run = new Run(text)
            {
                Background = SearchTermBackground,
                Foreground = SearchTermForeground
            };

            return run;
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

        private (int startOffset, int lengthOffset, int amountOfInlines) StoreSelection()
        {
            var start = Document.ContentStart.GetOffsetToPosition(Selection.Start);
            var length = Selection.Start.GetOffsetToPosition(Selection.End);
            var inlineCount = Document.Blocks.OfType<Paragraph>().FirstOrDefault()?.Inlines.Count ?? 0;

            return (start, length, inlineCount);
        }

        private ScrollViewer _scrollViewer;
        private Border _overlay;

        private double _overlayWidth;

        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
            "Icon", typeof(IconType), typeof(SearchTextBox), new PropertyMetadata(default(IconType)));

        public static readonly DependencyProperty LabelProperty = DependencyProperty.Register(
            "Label", typeof(string), typeof(SearchTextBox), new PropertyMetadata(default(string)));

        public static readonly DependencyProperty SearchCriteriaBackgroundProperty = DependencyProperty.Register(
            "SearchCriteriaBackground", typeof(Brush), typeof(SearchTextBox), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty SearchCriteriaForegroundProperty = DependencyProperty.Register(
            "SearchCriteriaForeground", typeof(Brush), typeof(SearchTextBox), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty SearchEngineProperty = DependencyProperty.Register(
            "SearchEngine", typeof(ISearchEngine), typeof(SearchTextBox), new PropertyMetadata(default(ISearchEngine)));

        public static readonly DependencyProperty SearchTermBackgroundProperty = DependencyProperty.Register(
            "SearchTermBackground", typeof(Brush), typeof(SearchTextBox), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty SearchTermForegroundProperty = DependencyProperty.Register(
            "SearchTermForeground", typeof(Brush), typeof(SearchTextBox), new PropertyMetadata(default(Brush)));
    }
}