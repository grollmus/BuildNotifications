using BuildNotifications.Core.Config;

namespace BuildNotifications.ViewModel.Utils.Configuration
{
    internal class AnimationSpeedSetter
    {
        private readonly IConfiguration _configuration;

        public AnimationSpeedSetter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void UpdateGlobalAnimationSpeed()
        {
            switch (_configuration.AnimationSpeed)
            {
                case AnimationMode.Disabled:
                    App.GlobalTweenHandler.TimeModifier = 99999;
                    break;
                case AnimationMode.DoubleSpeed:
                    App.GlobalTweenHandler.TimeModifier = 2.0;
                    break;
                default:
                    App.GlobalTweenHandler.TimeModifier = 1.0;
                    break;
            }
        }
    }
}