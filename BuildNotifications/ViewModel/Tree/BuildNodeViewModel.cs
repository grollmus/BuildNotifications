using System;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Tree
{
    internal class BuildNodeViewModel : BuildTreeNodeViewModel
    {
        private bool _isLargeSize;
        private bool _shouldBeLarge;
        private bool _isHighlighted;
        public IBuildNode Node { get; }

        public bool IsLargeSize
        {
            get => _isLargeSize;
            set
            {
                _isLargeSize = value;
                OnPropertyChanged();
            }
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

        private BuildStatus _buildStatus;

        public ICommand MouseEnterCommand { get; set; }
        public ICommand MouseLeaveCommand { get; set; }
        public ICommand MouseDownCommand { get; set; }
        public ICommand MouseUpCommand { get; set; }

        private BuildStatus Status
        {
            get { return _buildStatus; }
            set
            {
                _buildStatus = value;
                OnPropertyChanged(nameof(BuildStatus));
            }
        }

        public BuildNodeViewModel(IBuildNode node) : base(node)
        {
            Node = node;
            MouseEnterCommand = new DelegateCommand(OnMouseEnter);
            MouseLeaveCommand = new DelegateCommand(OnMouseLeave);
            MouseDownCommand = new DelegateCommand(OnMouseDown);
            MouseUpCommand = new DelegateCommand(OnMouseUp);

            Status = Node?.Build?.Status ?? BuildStatus.None;
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

        private void OnMouseDown(object obj)
        {
            Status = (BuildStatus) new Random().Next((int) BuildStatus.Cancelled, (int) BuildStatus.Failed + 1);
        }

        private void OnMouseUp(object obj)
        {
        }

        protected override BuildStatus CalculateBuildStatus() => Status;
        protected override string CalculateDisplayName() => "Build. Status: " + Status;
    }
}