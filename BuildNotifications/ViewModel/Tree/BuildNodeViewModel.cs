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

        public ICommand MouseEnterCommand { get; set; }
        public ICommand MouseLeaveCommand { get; set; }
        public IBuildNode Node { get; }

        public override void BackendPropertiesChanged()
        {
            OnPropertyChanged(nameof(BuildStatus));
        }

        protected override BuildStatus CalculateBuildStatus()
        {
            return Node?.Status ?? BuildStatus.None;
        }

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