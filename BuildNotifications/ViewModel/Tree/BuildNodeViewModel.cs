using System;
using System.Windows;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Utils;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.ViewModel.Tree
{
    public class BuildNodeViewModel : BuildTreeNodeViewModel
    {
        public BuildNodeViewModel(IBuildNode node)
            : base(node)
        {
            Node = node;
            MouseEnterCommand = new DelegateCommand(OnMouseEnter);
            MouseLeaveCommand = new DelegateCommand(OnMouseLeave);
            GoToBuildCommand = new DelegateCommand(x => Url.GoTo(Node.Build.Links.BuildWeb), x => Node.Build.Links.BuildWeb != null);
            GoToBranchCommand = new DelegateCommand(x => Url.GoTo(Node.Build.Links.BranchWeb), x => Node.Build.Links.BranchWeb != null);
            GoToDefinitionCommand = new DelegateCommand(x => Url.GoTo(Node.Build.Links.DefinitionWeb), x => Node.Build.Links.DefinitionWeb != null);
            BackendPropertiesChangedInternal();
        }

        public double ActualProgress
        {
            get => _actualProgress;
            set
            {
                _actualProgress = value;
                // Ensure the TweenHandler is only touched by a single thread.
                Application.Current.Dispatcher?.Invoke(() =>
                {
                    var globalTweenHandler = App.GlobalTweenHandler;
                    if (_progressTween != null && globalTweenHandler.Contains(_progressTween))
                        globalTweenHandler.Remove(_progressTween);

                    // always display at least 20%, so the user has a reasonable area to click on
                    var targetProgress = Node.Progress / 80.0 + 0.2;
                    _progressTween = this.Tween(x => x.ProgressToDisplay).To(targetProgress).In(5).Ease(Easing.Linear);
                    globalTweenHandler.Add(_progressTween);
                });
            }
        }

        public bool DisplayAsHollow
        {
            get => _displayAsHollow;
            set
            {
                _displayAsHollow = value;
                OnPropertyChanged();
            }
        }

        public bool IsFromPullRequest
        {
            get => _isFromPullRequest;
            set
            {
                _isFromPullRequest = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Toggle bool to tell the UI to shortly highlight this build.
        /// </summary>
        public bool DoShortHighlight
        {
            get => _doShortHighlight;
            set
            {
                _doShortHighlight = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoToBranchCommand { get; set; }
        public ICommand GoToBuildCommand { get; set; }
        public ICommand GoToDefinitionCommand { get; set; }

        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                _isHighlighted = value;
                OnPropertyChanged();
            }
        }

        private bool _isHighlightedBySight;

        public bool IsHighlightedBySight
        {
            get => _isHighlightedBySight;
            set
            {
                if (_isHighlightedBySight == value)
                    return;
                _isHighlightedBySight = value;
                OnPropertyChanged();
            }
        }

        public bool IsLargeSize
        {
            get => _isLargeSize;
            set
            {
                _isLargeSize = value;
                OnPropertyChanged();
            }
        }

        public ICommand MouseEnterCommand { get; set; }
        public ICommand MouseLeaveCommand { get; set; }
        public IBuildNode Node { get; }

        public double ProgressToDisplay
        {
            get => _progressToDisplay;
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1)
                    value = 1;

                if (Math.Abs(_progressToDisplay - value) < DoubleTolerance)
                    return;

                _progressToDisplay = value;
                OnPropertyChanged();
            }
        }

        public string RequestedBy => Node.Build.RequestedBy.DisplayName;

        public bool RequestedByIsSameAsFor => RequestedFor == RequestedBy;

        public string RequestedFor => Node.Build.RequestedFor?.DisplayName ?? RequestedBy;

        public bool IsManuallyRequestedByUser => RequestedByIsSameAsFor && Node.Build.IsRequestedByCurrentUser;

        public string StatusDisplayName => StringLocalizer.Instance[_buildStatus.ToString()];

        public int UserColumns => RequestedByIsSameAsFor ? 1 : 2;

        public override void BackendPropertiesChanged()
        {
            BackendPropertiesChangedInternal();
        }

        protected override BuildStatus CalculateBuildStatus()
        {
            return _buildStatus;
        }

        protected override DateTime CalculateChangedDate()
        {
            return _changedDate;
        }

        protected override string CalculateDisplayName()
        {
            return "Build. Status: " + BuildStatus;
        }

        protected override DateTime CalculateQueueTime()
        {
            return _queuedTime;
        }

        private void BackendPropertiesChangedInternal()
        {
            UpdateBuildStatus();
            UpdateChangedDate();
            UpdateQueuedDate();

            ActualProgress = Node.Progress;
            IsHighlightedBySight = Node.IsHighlightedBySight;
        }

        private void OnMouseEnter(object obj)
        {
            _shouldBeLarge = IsLargeSize;
            IsLargeSize = true;
        }

        private void OnMouseLeave(object obj)
        {
            IsLargeSize = _shouldBeLarge;
        }

        private void UpdateBuildStatus()
        {
            var newStatus = Node?.Status ?? BuildStatus.None;
            if (_buildStatus == newStatus)
                return;

            _buildStatus = newStatus;
            OnPropertyChanged(nameof(BuildStatus));
            ShortlyHighlightThisBuild();
        }

        private void UpdateChangedDate()
        {
            var newDate = Node?.LastChangedTime ?? DateTime.MinValue;
            if (_changedDate == newDate)
                return;

            _changedDate = newDate;
            OnPropertyChanged(nameof(ChangedDate));
        }

        private void UpdateQueuedDate()
        {
            var newDate = Node?.QueueTime ?? DateTime.MinValue;
            if (_queuedTime == newDate)
                return;

            _queuedTime = newDate;
            OnPropertyChanged(nameof(QueueTime));
        }

        private void ShortlyHighlightThisBuild()
        {
            DoShortHighlight = false;
            DoShortHighlight = true;
        }

        private Timeline? _progressTween;

        private BuildStatus _buildStatus;

        private DateTime _changedDate;
        private DateTime _queuedTime;

        private bool _isLargeSize;
        private bool _shouldBeLarge;
        private bool _isHighlighted;
        private double _progressToDisplay = 0.2;
        private bool _displayAsHollow;
        private double _actualProgress;
        private bool _doShortHighlight;
        private bool _isFromPullRequest;

        private const double DoubleTolerance = 0.0000000001;
    }
}