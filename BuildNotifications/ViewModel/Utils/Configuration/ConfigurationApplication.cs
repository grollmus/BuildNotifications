using BuildNotifications.Core.Config;

namespace BuildNotifications.ViewModel.Utils.Configuration
{
    /// <summary>
    /// Applies certain options from an configuration
    /// </summary>
    internal class ConfigurationApplication
    {
        public ConfigurationApplication(IConfiguration configuration)
        {
            _autostartHelper = new AutostartHelper(configuration);
            _animationSpeedSetter = new AnimationSpeedSetter(configuration);
            _themeSetter = new ThemeSetter(configuration);
        }

        public void ApplyChanges()
        {
            _autostartHelper.UpdateRegistrationForAutostart();
            _animationSpeedSetter.UpdateGlobalAnimationSpeed();
            _themeSetter.Apply();
        }

        private readonly AutostartHelper _autostartHelper;
        private readonly AnimationSpeedSetter _animationSpeedSetter;
        private readonly ThemeSetter _themeSetter;
    }
}