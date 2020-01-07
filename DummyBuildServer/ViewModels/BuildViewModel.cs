using BuildNotifications.Plugin.DummyBuildServer;

namespace DummyBuildServer.ViewModels
{
    internal class BuildViewModel : ViewModelBase
    {
        public BuildViewModel(Build build)
        {
            Build = build;
        }

        public Build Build { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value == _isSelected)
                    return;
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public int Progress
        {
            get => _progress;
            set
            {
                if (value == _progress)
                    return;
                _progress = value;
                OnPropertyChanged();
            }
        }

        public override string ToString()
        {
            return $"{Build.Id} - {Build.Definition.Name} on {Build.BranchName}";
        }

        private bool _isSelected;
        private int _progress;
    }
}