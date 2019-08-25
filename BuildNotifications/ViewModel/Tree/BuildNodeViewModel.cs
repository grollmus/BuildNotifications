using System;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Tree
{
    public class BuildNodeViewModel : BuildTreeNodeViewModel
    {
        public BuildNodeViewModel(IBuildNode node) : base(node)
        {
            Node = node;
            MouseEnterCommand = new DelegateCommand(OnMouseEnter);
            MouseLeaveCommand = new DelegateCommand(OnMouseLeave);
            UpdateBuildStatus();
            UpdateChangedDate();
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

        private BuildStatus _buildStatus;

        public ICommand MouseEnterCommand { get; set; }
        public ICommand MouseLeaveCommand { get; set; }
        public IBuildNode Node { get; }

        public override void BackendPropertiesChanged()
        {
            UpdateBuildStatus();
            UpdateChangedDate();
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
    }
}