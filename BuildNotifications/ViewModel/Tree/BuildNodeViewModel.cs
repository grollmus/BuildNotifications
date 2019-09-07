using System;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Utils;
using TweenSharp.Animation;
using TweenSharp.Factory;
using Timeline = TweenSharp.Animation.Timeline;

namespace BuildNotifications.ViewModel.Tree
{
    public class BuildNodeViewModel : BuildTreeNodeViewModel
    {
        public BuildNodeViewModel(IBuildNode node) : base(node)
        {
            Node = node;
            MouseEnterCommand = new DelegateCommand(OnMouseEnter);
            MouseLeaveCommand = new DelegateCommand(OnMouseLeave);
            BackendPropertiesChanged();
        }

        public bool IsHighlighted
        {
            get => _isHighlighted;
            set
            {
                _isHighlighted = value;
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

        private const double DoubleTolerance = 0.0000000001;

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

        public double ActualProgress
        {
            get => _actualProgress;
            set
            {
                _actualProgress = value;
                // Ensure the TweenHandler is only touched by a single thread.
                App.Current.Dispatcher.Invoke(() =>
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

        private Timeline _progressTween;

        public bool DisplayAsHollow
        {
            get => _displayAsHollow;
            set
            {
                _displayAsHollow = value;
                OnPropertyChanged();
            }
        }

        private BuildStatus _buildStatus;

        public ICommand MouseEnterCommand { get; set; }
        public ICommand MouseLeaveCommand { get; set; }
        public IBuildNode Node { get; }

        public override void BackendPropertiesChanged()
        {
            UpdateBuildStatus();
            UpdateChangedDate();

            ActualProgress = Node.Progress;
        }

        private void UpdateBuildStatus()
        {
            var newStatus = Node?.Status ?? BuildStatus.None;
            if (_buildStatus == newStatus)
                return;

            _buildStatus = newStatus;
            OnPropertyChanged(nameof(BuildStatus));
        }

        private DateTime _changedDate;

        private void UpdateChangedDate()
        {
            var newDate = Node?.LastChangedTime ?? DateTime.MinValue;
            if (_changedDate == newDate)
                return;

            _changedDate = newDate;
            OnPropertyChanged(nameof(ChangedDate));
        }

        protected override BuildStatus CalculateBuildStatus() => _buildStatus;

        protected override DateTime CalculateChangedDate() => _changedDate;

        protected override string CalculateDisplayName()
        {
            return "Build. Status: " + BuildStatus;
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

        private bool _isLargeSize;
        private bool _shouldBeLarge;
        private bool _isHighlighted;
        private double _progressToDisplay = 0.2;
        private bool _displayAsHollow;
        private double _actualProgress;
    }
}