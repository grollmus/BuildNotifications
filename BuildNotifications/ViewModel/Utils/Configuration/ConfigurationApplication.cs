using BuildNotifications.Core.Config;

namespace BuildNotifications.ViewModel.Utils.Configuration
{
    /// <summary>
    /// Applies certain options from an configuration
    /// </summary>
    internal class ConfigurationApplication
    {
        private readonly AutostartHelper _autostartHelper;
        private readonly AnimationSpeedSetter _animationSpeedSetter;

        public ConfigurationApplication(IConfiguration configuration)
        {
            _autostartHelper = new AutostartHelper(configuration);
            _animationSpeedSetter = new AnimationSpeedSetter(configuration);
        }

        public void ApplyChanges()
        {
            _autostartHelper.UpdateRegistrationForAutostart();
            _animationSpeedSetter.UpdateGlobalAnimationSpeed();
        }
    }
}
