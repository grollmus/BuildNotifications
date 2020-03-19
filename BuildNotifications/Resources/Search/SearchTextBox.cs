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
            KeyUp += OnKeyDown;

            // use dummy instances for a quick and easy way to avoid null reference exceptions
            _scrollViewer = new ScrollViewer();
            _overlay = new Border();
            _popup = new Popup();

            TextChanged += OnTextChanged;
            Loaded += OnLoaded;
            SelectionChanged += OnSelectionChanged;

            CurrentSuggestions.Add(new SearchSuggestionViewModel());
            CurrentSuggestions.Add(new SearchSuggestionViewModel());
            CurrentSuggestions.Add(new SearchSuggestionViewModel());
            CurrentSuggestions.Add(new SearchSuggestionViewModel());
        }

        [UsedImplicitly]
        public RemoveTrackingObservableCollection<SearchSuggestionViewModel> CurrentSuggestions { get; } = new RemoveTrackingObservableCollection<SearchSuggestionViewModel>();

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
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private (ISearchBlock block, bool caretIsInSearchCriteria) BlockOfCurrentSelection(IList<(Run createdRun, ISearchBlock forBlock, bool isSearchCriteria)> search)
        {
            var allInlines = Document.Blocks.OfType<Paragraph>().FirstOrDefault()?.Inlines ?? Enumerable.Empty<Inline>();

            var inlineOfCursor = allInlines.FirstOrDefault(x => x.ContentStart.CompareTo(Selection.Start) == -1 && x.ContentEnd.CompareTo(Selection.Start) == 1);
            foreach (var (createdRun, forBlock, isSearchCriteria) in search)
            {
                if (createdRun == inlineOfCursor)
                    return (forBlock, isSearchCriteria);
            }

            var lastTuple = search.Last();
            return (lastTuple.forBlock, lastTuple.isSearchCriteria);
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

        private IEnumerable<(Run createdRun, ISearchBlock forBlock, bool isSearchCriteria)> DisplaySearchBlocks(IEnumerable<ISearchBlock> parsedBlocks)
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
                    {
                        var run = DefaultRun(parsedBlock.SearchedText);
                        inlines.Add(run);
                        yield return (run, parsedBlock, false);
                    }
                }
                else
                {
                    var searchCriteriaRun = SearchCriteriaRun($"{parsedBlock.SearchCriteria.LocalizedKeyword}:");
                    inlines.Add(searchCriteriaRun);
                    yield return (searchCriteriaRun, parsedBlock, true);

                    if (!string.IsNullOrEmpty(parsedBlock.SearchedText))
                    {
                        var searchTermRun = SearchTermRun(parsedBlock.SearchedText);
                        inlines.Add(searchTermRun);
                        yield return (searchTermRun, parsedBlock, false);
                    }
                }
            }
        }

        private void DisplaySuggestions((ISearchBlock block, bool caretIsInSearchCriteria) blockOfCaret)
        {
            var (block, caretIsInSearchCriteria) = blockOfCaret;

            CurrentSuggestions.Clear();
            if (SearchCriteriaViewModel?.SearchCriteria != block.SearchCriteria)
                SearchCriteriaViewModel = SearchBlockViewModel.FromSearchCriteria(block.SearchCriteria);
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
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            CheckForScrollViewerCollision();
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

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchEngine == null)
                return;

            var currentText = CurrentText();
            var search = SearchEngine.Parse(currentText);

            TextChanged -= OnTextChanged;
            var storedSelection = StoreSelection();

            var createdRuns = DisplaySearchBlocks(search.Blocks);
            _runsForCurrentSearchBlocks.Clear();
            var valueTuples = createdRuns.ToList();
            foreach (var keyValuePair in createdRuns)
            {
                _runsForCurrentSearchBlocks.Add(keyValuePair);
            }

            RestoreSelection(storedSelection);
            TextChanged += OnTextChanged;

            DisplaySuggestionsForCaret();
        }

        private void OnWindowLocationChanged(object? sender, EventArgs e) => UpdatePopupPlacement();

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
    }

    internal class SearchBlockViewModel : BaseViewModel
    {
        public ISearchCriteria SearchCriteria { get; }

        protected SearchBlockViewModel(ISearchCriteria searchCriteria)
        {
            SearchCriteria = searchCriteria;
        }

        public static SearchBlockViewModel FromSearchCriteria(ISearchCriteria searchCriteria)
        {
            if (searchCriteria is DefaultSearchCriteria defaultSearchCriteria)
                return new DefaultSearchBlockViewModel(defaultSearchCriteria);
            else
            {
                var searchBlockViewModel = new SearchBlockViewModel(searchCriteria);

                foreach (var example in searchCriteria.LocalizedExamples)
                {
                    searchBlockViewModel.Examples.Add(new SearchBlockExampleViewModel($" {searchCriteria.LocalizedKeyword}: ", example));
                }

                return searchBlockViewModel;
            }
        }

        public string Description => SearchCriteria.LocalizedDescription;

        public string Keyword => SearchCriteria.LocalizedKeyword;

        [UsedImplicitly]
        public ObservableCollection<SearchBlockExampleViewModel> Examples { get; } = new ObservableCollection<SearchBlockExampleViewModel>();
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
        public string Keyword { get; }
        public string ExampleText { get; }

        public SearchBlockExampleViewModel(string keyword, string exampleText)
        {
            Keyword = keyword;
            ExampleText = exampleText;
        }
    }
}