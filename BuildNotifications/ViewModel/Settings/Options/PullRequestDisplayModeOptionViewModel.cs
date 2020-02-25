using BuildNotifications.Core.Config;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public class PullRequestDisplayModeOptionViewModel : EnumOptionBaseViewModel<PullRequestDisplayMode>
    {
        public PullRequestDisplayModeOptionViewModel(string displayName, PullRequestDisplayMode value = default)
            : base(displayName, value)
        {
        }
    }
}