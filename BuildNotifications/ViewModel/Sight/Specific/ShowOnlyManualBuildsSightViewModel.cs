using BuildNotifications.Core.Pipeline.Tree.Sight;
using BuildNotifications.Resources.Icons;

namespace BuildNotifications.ViewModel.Sight.Specific
{
    internal class ShowOnlyManualBuildsSightViewModel : BaseSightViewModel
    {
        public ShowOnlyManualBuildsSightViewModel() : base(new ManualBuildsSight(), IconType.ManualBuild, "ManualBuildsSightToolTip")
        {
        }
    }
}