using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.Resources.Icons;
using JetBrains.Annotations;

namespace BuildNotifications.Resources.Search
{
    internal class SearchTextBox : RichTextBox, INotifyPropertyChanged
    {
        public SearchTextBox()
        {
            GotFocus += OnGotFocus;
            LostFocus += OnLostFocus;
            PreviewMouseDown += OnPreviewMouseDown;
            PreviewKeyDown += OnKeyDown;
            PreviewKeyUp += OnKeyUp;
            SizeChanged += OnSizeChanged;

            // use dummy instances for a quick and easy way to avoid null reference exceptions
            _scrollViewer = new ScrollViewer();
            _overlay = new Border();
            _popup = new Popup();
            
            LostKeyboardFocus += (sender, args) => _popup.IsOpen = false;

            ListenToTextEvents();
            Loaded += OnLoaded;
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

        public SearchBlockViewModel? SearchCriteriaViewModel
        {
            get => _searchCriteriaViewModel;
            set
            {
                _searchCriteriaViewModel = value;
                OnPropertyChanged();
            }
        }

        public ISearchEngine? SearchEngine
        {
            get => (ISearchEngine) GetValue(SearchEngineProperty);
            set => SetValue(SearchEngineProperty, value);
        }

        public ISearchHistory? SearchHistory
        {
            get => (ISearchHistory) GetValue(SearchHistoryProperty);
            set => SetValue(SearchHistoryProperty, value);
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
            _popup = GetTemplateChild("PART_Popup") as Popup ?? new Popup();

            base.OnApplyTemplate();
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ApplySuggestion(SearchSuggestionViewModel selectedSuggestion)
        {
            if (!_runsForCurrentSearchBlocks.Any())
                return;

            var storedSelection = StoreSelection();
            var (_, runOfBlock, __) = BlockOfCurrentSelection(_runsForCurrentSearchBlocks);
            int indicesToMoveCaret;

            var suggestionText = selectedSuggestion.SuggestedText;

            // if the suggestion isn't a search criteria (end with the keyword separator), add the general separator so a new search term is automatically opened for the user
            const char specificToGeneralSeparator = ',';
            const char keywordSeparator = ':';

            // apply spaces for a cleaner appearance
            if (!selectedSuggestion.IsKeyword && !selectedSuggestion.IsFromHistory)
                suggestionText = ' ' + suggestionText;

            // add automatic separator for better ease-of-use
            if (!suggestionText.EndsWith(keywordSeparator) && !selectedSuggestion.IsFromHistory)
                suggestionText += specificToGeneralSeparator;
            else
                suggestionText += ' ';

            // if the caret is on the end of the text and there is no run
            if (runOfBlock == null)
            {
                var inlines = Document.Blocks.OfType<Paragraph>().FirstOrDefault()?.Inlines;

                inlines?.Add(DefaultRun(suggestionText));
                indicesToMoveCaret = suggestionText.Length;
            }
            else // if the caret is somewhere within a search term
            {
                var indexOfCaretInRun = runOfBlock.ContentStart.GetOffsetToPosition(Selection.Start);
                runOfBlock.Text = suggestionText;
                indicesToMoveCaret = suggestionText.Length - indexOfCaretInRun;
            }

            storedSelection = (storedSelection.startOffset + indicesToMoveCaret, storedSelection.lengthOffset, storedSelection.amountOfInlines, storedSelection.caretPosition);
            if (!IsFocused)
                Focus();
            RestoreSelection(storedSelection);
            InvalidateVisual();
            Invalidate(this);
        }

        private void Invalidate(DependencyObject dependencyObject)
        {
            var parent = VisualTreeHelper.GetParent(dependencyObject);
            var childCount = VisualTreeHelper.GetChildrenCount(dependencyObject);
            for (var i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(dependencyObject, i);
                if (child is UIElement childUiElement)
                    childUiElement.InvalidateVisual();
            }

            if (parent is UIElement uiElement)
            {
                uiElement.InvalidateVisual();
                Invalidate(parent);
            }
        }

        private (ISearchBlock block, Run? inlineOfCaret, bool inlineIsSearchCriteria) BlockOfCurrentSelection(IList<(Run createdRun, ISearchBlock forBlock, bool isSearchCriteria)> search)
        {
            var allInlines = (Document.Blocks.OfType<Paragraph>().FirstOrDefault()?.Inlines.OfType<Run>() ?? Enumerable.Empty<Run>()).ToList();

            // the run which contains the caret is ambiguous as the caret might be between two runs.
            // additionally the "direction" of the caret also matters. As a workaround the positions left and right of the caret are used
            // the right one will usually be the one the user expects to write in and is therefore used.
            // However, between different runs there are "dead" spaces in which the caret cannot be.
            // This is relevant when the caret is at the end of the search field and the run right to it is empty.
            // In this case the caret is within no run (as it cannot be within the empty one) and the position LEFT to it, will touch the run to the left.
            // However, since the user usually writes to the right, we try to reach further right

            var rightOfCaret = CaretPosition.GetPositionAtOffset(1, LogicalDirection.Forward) ?? CaretPosition;
            var inlineOfCursor = allInlines.FirstOrDefault(x => x.ContentStart.CompareTo(rightOfCaret) <= 0 && x.ContentEnd.CompareTo(rightOfCaret) >= 0);

            if (inlineOfCursor == null)
            {
                var leftOfCaret = CaretPosition.GetPositionAtOffset(-1, LogicalDirection.Backward) ?? CaretPosition;
                inlineOfCursor = allInlines.FirstOrDefault(x => x.ContentStart.CompareTo(leftOfCaret) <= 0 && x.ContentEnd.CompareTo(leftOfCaret) == 0);

                if (inlineOfCursor == null)
                {
                    rightOfCaret = CaretPosition.GetPositionAtOffset(2, LogicalDirection.Forward) ?? CaretPosition;
                    inlineOfCursor = allInlines.FirstOrDefault(x => x.ContentStart.CompareTo(rightOfCaret) <= 0 && x.ContentEnd.CompareTo(rightOfCaret) >= 0) ?? allInlines.Last();
                }
            }

            foreach (var (createdRun, forBlock, isSearchCriteria) in search)
            {
                if (createdRun == inlineOfCursor)
                    return (forBlock, createdRun, isSearchCriteria);
            }

            var lastTuple = search.Last();
            return (lastTuple.forBlock, null, true);
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

        private IEnumerable<(Run createdRun, ISearchBlock forBlock, bool isSearchCriteria)> DisplaySearchBlocks(ICollection<ISearchBlock> parsedBlocks)
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
                if (string.IsNullOrEmpty(parsedBlock.SearchCriteria.LocalizedKeyword(CultureInfo.CurrentCulture)))
                {
                    var run = DefaultRun(parsedBlock.EnteredText);
                    inlines.Add(run);
                    yield return (run, parsedBlock, false);
                }
                else
                {
                    var searchCriteriaRun = SearchCriteriaRun($"{parsedBlock.SearchCriteria.LocalizedKeyword(CultureInfo.CurrentCulture)}:");
                    inlines.Add(searchCriteriaRun);
                    yield return (searchCriteriaRun, parsedBlock, true);

                    var searchTermRun = SearchTermRun(parsedBlock.EnteredText);
                    inlines.Add(searchTermRun);
                    yield return (searchTermRun, parsedBlock, false);
                }
            }
        }

        private void DisplaySuggestions((ISearchBlock block, Run? inlineOfCaret, bool inlineIsSearchCriteria) blockOfCaret)
        {
            var (block, _, isSearchCriteria) = blockOfCaret;

            if (SearchCriteriaViewModel?.SearchCriteria != block.SearchCriteria)
                SearchCriteriaViewModel = SearchBlockViewModel.FromSearchCriteria(block.SearchCriteria, SearchHistory ?? new EmptySearchHistory(), ApplySuggestion);

            if (isSearchCriteria)
                SearchCriteriaViewModel.ClearSuggestions();
            else
                SearchCriteriaViewModel.UpdateSuggestions(block.SearchedTerm);
        }

        private void DisplaySuggestionsForCaret()
        {
            if (!_runsForCurrentSearchBlocks.Any())
                return;
            var blockOfCurrentSelection = BlockOfCurrentSelection(_runsForCurrentSearchBlocks);
            DisplaySuggestions(blockOfCurrentSelection);
        }

        private void ListenToTextEvents()
        {
            StopListeningToTextEvents();
            TextChanged += OnTextChanged;
            SelectionChanged += OnSelectionChanged;
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            CheckForScrollViewerCollision();
            ParseCurrentText();
            _popup.IsOpen = true;
            SelectAll();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!_popup.IsOpen)
                _popup.IsOpen = true;

            CheckForScrollViewerCollision();

            if (SearchCriteriaViewModel == null)
                return;

            switch (e.Key)
            {
                case Key.Down:
                case Key.Enter:
                case Key.Up:
                    e.Handled = true;
                    if (_lastPressedKey == e.Key) // avoid multiple key inputs for this action
                        return;

                    _lastPressedKey = e.Key;
                    break;
                case Key.Escape:
                    var request = new TraversalRequest(FocusNavigationDirection.Next);
                    request.Wrapped = true;
                    MoveFocus(request);
                    e.Handled = true;
                    return;
                default:
                    return;
            }

            HandleKeyDownForSuggestions(e);
        }

        private void HandleKeyDownForSuggestions(KeyEventArgs e)
        {
            if (SearchCriteriaViewModel == null)
                return;

            switch (e.Key)
            {
                case Key.Down:
                    SearchCriteriaViewModel.ChangeSelectedSuggestionIndex(1);
                    break;
                case Key.Up:
                    SearchCriteriaViewModel.ChangeSelectedSuggestionIndex(-1);
                    break;
                case Key.Enter:
                    if (SearchCriteriaViewModel.SelectedSuggestion != null)
                    {
                        e.Handled = true;
                        ApplySuggestion(SearchCriteriaViewModel.SelectedSuggestion);
                    }

                    break;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Down:
                case Key.Enter:
                case Key.Up:
                    if (_lastPressedKey == e.Key)
                        _lastPressedKey = null;
                    break;
                default:
                    return;
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            var windowThisControlIsIn = System.Windows.Window.GetWindow(this);
            if (windowThisControlIsIn != null)
                windowThisControlIsIn.LocationChanged += OnWindowLocationChanged;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            ResetScrollViewerMargin();
            _popup.IsOpen = false;
            if (SearchHistory != null && !string.IsNullOrEmpty(_currentSearchTerm))
                SearchHistory.AddEntry(_currentSearchTerm);
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

        private void OnSelectionChanged(object sender, RoutedEventArgs e) => DisplaySuggestionsForCaret();

        private void OnSizeChanged(object? sender, EventArgs e) => UpdatePopupPlacement();

        private void OnTextChanged(object sender, TextChangedEventArgs e) => ParseCurrentText();

        private void OnWindowLocationChanged(object? sender, EventArgs e)
        {
            UpdatePopupPlacement();
        }

        private void ParseCurrentText()
        {
            if (SearchEngine == null)
                return;

            var currentText = CurrentText();
            var search = SearchEngine.Parse(currentText);
            _currentSearchTerm = search.SearchedTerm;

            StopListeningToTextEvents();
            var storedSelection = StoreSelection();

            var createdRuns = DisplaySearchBlocks(search.Blocks.ToList());
            _runsForCurrentSearchBlocks.Clear();
            foreach (var keyValuePair in createdRuns)
            {
                _runsForCurrentSearchBlocks.Add(keyValuePair);
            }

            RestoreSelection(storedSelection);
            TextChanged += OnTextChanged;

            DisplaySuggestionsForCaret();
            ListenToTextEvents();
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

        private void RestoreSelection((int startOffset, int lengthOffset, int amountOfInlines, int caretPosition) storedSelection)
        {
            // when restoring do not ignore empty inlines. I am honestly not sure why
            var inlineCountNow = Document.Blocks.OfType<Paragraph>().FirstOrDefault()?.Inlines.Count ?? 0;
            var additionalCountForInlines = (inlineCountNow - storedSelection.amountOfInlines) * 2;

            var start = Document.ContentStart.GetPositionAtOffset(storedSelection.startOffset + additionalCountForInlines);
            var end = start?.GetPositionAtOffset(storedSelection.lengthOffset);

            if (start == null || end == null)
                return;

            Selection.Select(start!, end);
            for (var i = 0; i <= 2; i++)
            {
                CaretPosition = start.GetPositionAtOffset(i) ?? start;
                if (CaretPosition == null || Document.ContentStart.GetOffsetToPosition(CaretPosition) == Document.ContentStart.GetOffsetToPosition(start!))
                    break;
            }
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

        private void StopListeningToTextEvents()
        {
            TextChanged -= OnTextChanged;
            SelectionChanged -= OnSelectionChanged;
        }

        private (int startOffset, int lengthOffset, int amountOfInlines, int caretPosition) StoreSelection()
        {
            var start = Document.ContentStart.GetOffsetToPosition(Selection.Start);
            var caret = Document.ContentStart.GetOffsetToPosition(CaretPosition);
            var length = Selection.Start.GetOffsetToPosition(Selection.End);
            var inlineCount = Document.Blocks.OfType<Paragraph>().FirstOrDefault()?.Inlines.Count ?? 0;

            return (start, length, inlineCount, caret);
        }

        private void UpdatePopupPlacement()
        {
            // force a redraw
            var offset = _popup.HorizontalOffset;
            _popup.HorizontalOffset = offset + 1;
            _popup.HorizontalOffset = offset;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private readonly IList<(Run createdRun, ISearchBlock forBlock, bool isSearchCriteria)> _runsForCurrentSearchBlocks = new List<(Run createdRun, ISearchBlock forBlock, bool isSearchCriteria)>();

        private SearchBlockViewModel? _searchCriteriaViewModel;

        private ScrollViewer _scrollViewer;
        private Border _overlay;
        private Popup _popup;
        private Key? _lastPressedKey;
        private string _currentSearchTerm = string.Empty;

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

        public static readonly DependencyProperty SearchHistoryProperty = DependencyProperty.Register(
            "SearchHistory", typeof(ISearchHistory), typeof(SearchTextBox), new PropertyMetadata(default(ISearchHistory)));

        public static readonly DependencyProperty SearchTermBackgroundProperty = DependencyProperty.Register(
            "SearchTermBackground", typeof(Brush), typeof(SearchTextBox), new PropertyMetadata(default(Brush)));

        public static readonly DependencyProperty SearchTermForegroundProperty = DependencyProperty.Register(
            "SearchTermForeground", typeof(Brush), typeof(SearchTextBox), new PropertyMetadata(default(Brush)));
    }
}