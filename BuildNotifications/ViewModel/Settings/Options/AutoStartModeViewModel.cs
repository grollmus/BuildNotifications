using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public class AutoStartModeViewModel : EnumOptionBaseViewModel<AutostartMode>
    {
        public AutoStartModeViewModel(AutostartMode value)
            : base(StringLocalizer.Keys.Autostart, value)
        {
        }
    }
}