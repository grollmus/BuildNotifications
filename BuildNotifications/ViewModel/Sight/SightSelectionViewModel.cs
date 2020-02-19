using BuildNotifications.ViewModel.Tree;

namespace BuildNotifications.ViewModel.Sight
{
    public class SightSelectionViewModel : BaseViewModel
    {
        private BuildTreeViewModel? _buildTree;

        public BuildTreeViewModel? BuildTree
        {
            get => _buildTree;
            set
            {
                _buildTree = value;
                UpdateBuildSights();
            }
        }

        private void UpdateBuildSights()
        {
        }

        public bool? HighlightManualBuilds { get; set; }
    }
}