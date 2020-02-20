using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;

namespace BuildNotifications.ViewModel.Settings.Options
{
    public class AnimationsOptionViewModel : EnumOptionBaseViewModel<AnimationMode>
    {
        public AnimationsOptionViewModel(AnimationMode value)
            : base(StringLocalizer.Keys.AnimationSpeed, value)
        {
        }
    }
}