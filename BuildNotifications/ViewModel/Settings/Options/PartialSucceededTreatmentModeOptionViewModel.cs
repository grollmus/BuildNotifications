using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public class PartialSucceededTreatmentModeOptionViewModel : EnumOptionBaseViewModel<PartialSucceededTreatmentMode>
    {
        public PartialSucceededTreatmentModeOptionViewModel(PartialSucceededTreatmentMode value)
            : base(StringLocalizer.Keys.PartialSucceededTreatmentMode, value)
        {
        }
    }
}