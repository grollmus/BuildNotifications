using System;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Tree
{
    internal class BuildNodeViewModel : BuildTreeNodeViewModel
    {
        public BuildNodeViewModel(IBuildNode node) : base(node)
        {
            Node = node;
            MouseEnterCommand = new DelegateCommand(OnMouseEnter);
            MouseLeaveCommand = new DelegateCommand(OnMouseLeave);
            MouseDownCommand = new DelegateCommand(OnMouseDown);
            MouseUpCommand = new DelegateCommand(OnMouseUp);

            Status = Node?.Build?.Status ?? BuildStatus.None;
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

        public ICommand MouseDownCommand { get; set; }

        public ICommand MouseEnterCommand { get; set; }
        public ICommand MouseLeaveCommand { get; set; }
        public ICommand MouseUpCommand { get; set; }
        public IBuildNode Node { get; }

        private BuildStatus Status
        {
            get => _buildStatus;
            set
            {
                _buildStatus = value;
                OnPropertyChanged(nameof(BuildStatus));
            }
        }

        protected override BuildStatus CalculateBuildStatus()
        {
            return Status;
        }

        protected override string CalculateDisplayName()
        {
            return "Build. Status: " + Status;
        }

        private void OnMouseDown(object obj)
        {
            Status = (BuildStatus) new Random().Next((int) BuildStatus.Cancelled, (int) BuildStatus.Failed + 1);
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

        private void OnMouseUp(object obj)
        {
        }

        private bool _isLargeSize;
        private bool _shouldBeLarge;
        private bool _isHighlighted;

        private BuildStatus _buildStatus;
    }
}