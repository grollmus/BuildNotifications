using BuildNotifications.Core.Config;

namespace BuildNotifications.ViewModel.Utils
{
    /// <summary>
    /// Applies certain options from an configuration
    /// </summary>
    internal class ConfigurationDoer
    {
        private readonly AutostartHelper _autostartHelper;

        public ConfigurationDoer(IConfiguration configuration)
        {
            _autostartHelper = new AutostartHelper(configuration);
        }

        public void ApplyChanges()
        {
            _autostartHelper.UpdateRegistrationForAutostart();
        }
    }
}
