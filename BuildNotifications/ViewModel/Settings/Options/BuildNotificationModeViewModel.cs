using BuildNotifications.Core.Config;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public class BuildNotificationModeViewModel : EnumOptionBaseViewModel<BuildNotificationMode>
    {
        public BuildNotificationModeViewModel(BuildNotificationMode value, string displayName)
            : base(displayName, value)
        {
        }
    }
}