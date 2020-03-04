using BuildNotifications.Core.Pipeline.Tree.Sight;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.Sight.Specific
{
    internal class HighlightMyBuildsSightViewModel : BaseSightViewModel
    {
        public HighlightMyBuildsSightViewModel() : base(new MyBuildsSight(), IconType.MyBuild, "MyBuildsSightToolTip")
        {
        }
    }
}
