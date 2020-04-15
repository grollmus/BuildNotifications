using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Anotar.NLog;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.PluginInterfaces.Builds.Search;
using BuildNotifications.Resources.Icons;
using BuildNotifications.ViewModel;
using BuildNotifications.ViewModel.Utils;
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

            TextChanged += OnTextChanged;
            Loaded += OnLoaded;
            SelectionChanged += OnSelectionChanged;
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

        private (ISearchBlock block, Run? inlineOfCaret) BlockOfCurrentSelection(IList<(Run createdRun, ISearchBlock forBlock, bool isSearchCriteria)> search, bool ignoreRunsOfSearchCriteria = false)
        {
            var allInlines = (Document.Blocks.OfType<Paragraph>().FirstOrDefault()?.Inlines.OfType<Run>() ?? Enumerable.Empty<Run>()).ToList();

            var inlineOfCursor = allInlines.FirstOrDefault(x => x.ContentStart.CompareTo(Selection.Start) <= 0 && x.ContentEnd.CompareTo(Selection.Start) >= 0);

            var start = Document.ContentStart.GetOffsetToPosition(Selection.Start);
            var length = Selection.Start.GetOffsetToPosition(Selection.End);
            var End = Document.ContentStart.GetOffsetToPosition(Selection.End);
            LogTo.Debug($"\n\nSELECTION START: {start} End:{End} Length:{length}");
            LogTo.Debug(string.Join("\n", allInlines.Select(x => $"INLINE: \"{x.Text}\" Start: {Document.ContentStart.GetOffsetToPosition(x.ContentStart)} End: {Document.ContentStart.GetOffsetToPosition(x.ContentEnd)}")));

            foreach (var (createdRun, forBlock, isSearchCriteria) in search)
            {
                if (createdRun == inlineOfCursor && (!ignoreRunsOfSearchCriteria || !isSearchCriteria))
                    return (forBlock, createdRun);
            }

            var lastTuple = search.Last();
            return (lastTuple.forBlock, null);
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
                if (string.IsNullOrEmpty(parsedBlock.SearchCriteria.LocalizedKeyword))
                {
                    // Omit empty default blocks. Except if default is the first block. This is the case when the entire search term is an empty string

                    var run = DefaultRun(parsedBlock.SearchedText);
                    inlines.Add(run);
                    yield return (run, parsedBlock, false);
                }
                else
                {
                    var searchCriteriaRun = SearchCriteriaRun($"{parsedBlock.SearchCriteria.LocalizedKeyword}:");
                    inlines.Add(searchCriteriaRun);
                    yield return (searchCriteriaRun, parsedBlock, true);

                    var searchTermRun = SearchTermRun(parsedBlock.SearchedText);
                    inlines.Add(searchTermRun);
                    yield return (searchTermRun, parsedBlock, false);
                }
            }
        }

        private void DisplaySuggestions((ISearchBlock block, Run? inlineOfCaret) blockOfCaret)
        {
            var (block, _) = blockOfCaret;

            if (SearchCriteriaViewModel?.SearchCriteria != block.SearchCriteria)
                SearchCriteriaViewModel = SearchBlockViewModel.FromSearchCriteria(block.SearchCriteria);

            SearchCriteriaViewModel.UpdateSuggestions(block.SearchedText);
        }

        private void DisplaySuggestionsForCaret()
        {
            if (!_runsForCurrentSearchBlocks.Any())
                return;
            var blockOfCurrentSelection = BlockOfCurrentSelection(_runsForCurrentSearchBlocks);
            DisplaySuggestions(blockOfCurrentSelection);
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            CheckForScrollViewerCollision();
            if (string.IsNullOrWhiteSpace(Selection.ToString()))
                SelectAll();

            _overlayWidth = RenderSizeOfElement(_overlay).Width;
            ParseCurrentText();
            _popup.IsOpen = true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
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
                default:
                    return;
            }

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

        private void ApplySuggestion(SearchSuggestionViewModel selectedSuggestion)
        {
            if (!_runsForCurrentSearchBlocks.Any())
                return;

            var storedSelection = StoreSelection();
            var (_, runOfBlock) = BlockOfCurrentSelection(_runsForCurrentSearchBlocks, true);
            int indicesToMoveCaret;

            var suggestionText = selectedSuggestion.SuggestedText;

            // if the suggestion isn't a search criteria (end with the keyword separator), add the general separator so a new search term is automatically opened for the user
            const char specificToGeneralSeparator = ',';
            const char keywordSeparator = ':';

            if (!suggestionText.EndsWith(keywordSeparator))
                suggestionText += specificToGeneralSeparator;

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

            storedSelection = (storedSelection.startOffset + indicesToMoveCaret, storedSelection.lengthOffset, storedSelection.amountOfInlines);
            RestoreSelection(storedSelection);
            DisplaySuggestionsForCaret();
        }

        private void OnSizeChanged(object? sender, EventArgs e) => UpdatePopupPlacement();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            var windowThisControlIsIn = System.Windows.Window.GetWindow(this);
            if (windowThisControlIsIn != null)
                windowThisControlIsIn.LocationChanged += OnWindowLocationChanged;
        }

        private void OnLostFocus(object sender, RoutedEventArgs e) => ResetScrollViewerMargin();

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

        private void OnTextChanged(object sender, TextChangedEventArgs e) => ParseCurrentText();

        private void ParseCurrentText()
        {
            if (SearchEngine == null)
                return;

            var currentText = CurrentText();
            var search = SearchEngine.Parse(currentText);

            TextChanged -= OnTextChanged;
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
        }

        private void OnWindowLocationChanged(object? sender, EventArgs e)
        {
            UpdatePopupPlacement();
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
            // when restoring do not ignore empty inlines. I am honestly not sure why, but 
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

    internal class SearchSuggestionViewModel : BaseViewModel
    {
        private readonly ISearchCriteriaSuggestion _searchCriteriaSuggestion;
        public string SuggestedText { get; }

        public SearchSuggestionViewModel(ISearchCriteriaSuggestion searchCriteriaSuggestion)
        {
            _searchCriteriaSuggestion = searchCriteriaSuggestion;
            SuggestedText = _searchCriteriaSuggestion.Suggestion;
        }

        public bool IsSameSuggestion(SearchSuggestionViewModel otherViewModel)
        {
            return otherViewModel._searchCriteriaSuggestion.Suggestion.Equals(_searchCriteriaSuggestion.Suggestion, StringComparison.InvariantCulture);
        }
    }

    internal class SearchBlockViewModel : BaseViewModel
    {
        protected SearchBlockViewModel(ISearchCriteria searchCriteria)
        {
            SearchCriteria = searchCriteria;
        }

        public string Description => SearchCriteria.LocalizedDescription;

        public ObservableCollection<SearchBlockExampleViewModel> Examples { get; } = new ObservableCollection<SearchBlockExampleViewModel>();

        public RemoveTrackingObservableCollection<SearchSuggestionViewModel> Suggestions { get; } = new RemoveTrackingObservableCollection<SearchSuggestionViewModel>(TimeSpan.FromMilliseconds(100));

        private SearchSuggestionViewModel? _selectedSuggestion;

        public SearchSuggestionViewModel? SelectedSuggestion
        {
            get => _selectedSuggestion;
            set
            {
                _selectedSuggestion = value;
                OnPropertyChanged();
            }
        }

        private int _selectedSuggestionIndex;

        public int SelectedSuggestionIndex
        {
            get => _selectedSuggestionIndex;
            set
            {
                _selectedSuggestionIndex = value;
                OnPropertyChanged();
            }
        }

        public string Keyword => SearchCriteria.LocalizedKeyword;
        public ISearchCriteria SearchCriteria { get; }

        public static SearchBlockViewModel FromSearchCriteria(ISearchCriteria searchCriteria)
        {
            if (searchCriteria is DefaultSearchCriteria defaultSearchCriteria)
                return new DefaultSearchBlockViewModel(defaultSearchCriteria);
            var searchBlockViewModel = new SearchBlockViewModel(searchCriteria);

            foreach (var example in searchCriteria.LocalizedExamples)
            {
                searchBlockViewModel.Examples.Add(new SearchBlockExampleViewModel($" {searchCriteria.LocalizedKeyword}: ", example));
            }

            return searchBlockViewModel;
        }

        public void UpdateSuggestions(string currentSearchTerm)
        {
            var newSuggestions = SearchCriteria.Suggest(currentSearchTerm).Select(s => new SearchSuggestionViewModel(s)).ToList();

            var toRemove = Suggestions.Where(s => !newSuggestions.Any(n => n.IsSameSuggestion(s))).ToList();
            var toAdd = newSuggestions.Where(n => !Suggestions.Any(s => s.IsSameSuggestion(n))).ToList();

            foreach (var suggestionToRemove in toRemove)
            {
                Suggestions.Remove(suggestionToRemove);
            }

            foreach (var suggestionToAdd in toAdd)
            {
                Suggestions.Add(suggestionToAdd);
            }

            Suggestions.Sort(s => newSuggestions.IndexOf(s));

            if (Suggestions.Any(s => !s.IsRemoving) && (SelectedSuggestion == null || SelectedSuggestion.IsRemoving))
                SelectedSuggestion = Suggestions.First(s => !s.IsRemoving);
        }

        public void ChangeSelectedSuggestionIndex(int indexChange)
        {
            var newIndex = SelectedSuggestionIndex + indexChange;
            if (newIndex < 0)
                newIndex = Suggestions.Count - 1;

            if (newIndex >= Suggestions.Count)
                newIndex = Suggestions.Count == 0 ? -1 : 0;

            SelectedSuggestionIndex = newIndex;
        }
    }

    internal class DefaultSearchBlockViewModel : SearchBlockViewModel
    {
        public DefaultSearchBlockViewModel(DefaultSearchCriteria defaultSearchCriteria) : base(defaultSearchCriteria)
        {
            foreach (var (keyword, exampleTerm) in defaultSearchCriteria.ExamplesFromEachSubCriteria())
            {
                Examples.Add(new SearchBlockExampleViewModel($" {keyword}: ", exampleTerm));
            }
        }
    }

    internal class SearchBlockExampleViewModel : BaseViewModel
    {
        public SearchBlockExampleViewModel(string keyword, string exampleText)
        {
            Keyword = keyword;
            ExampleText = exampleText;
        }

        public string ExampleText { get; }
        public string Keyword { get; }
    }
}