using BuildNotifications.Core.Config;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public class BuildNotificationModeViewModel : EnumOptionBaseViewModel<BuildNotificationModes>
    {
        public BuildNotificationModeViewModel(BuildNotificationModes value, string displayName)
            : base(displayName, value)
        {
        }
    }
}