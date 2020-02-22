using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Plugin;
using BuildNotifications.ViewModel.Utils;
using ReflectSettings;

namespace BuildNotifications.ViewModel.Settings
{
    public class SettingsViewModel_Legacy
    {
// properties *are* initialized within the constructor. However by a method call, which is not correctly recognized by the code analyzer yet.
#pragma warning disable CS8618 // warning about uninitialized non-nullable properties
        public SettingsViewModel_Legacy(IConfiguration configuration, Action saveMethod, IPluginRepository pluginRepository)
#pragma warning restore CS8618
        {
            Configuration = configuration;
            _saveMethod = saveMethod;
            _pluginRepository = pluginRepository;
            EditConnectionsCommand = new DelegateCommand(OnEditConnections);

            UpdateUser();
        }

        public IConfiguration Configuration { get; }

        public BaseViewModel ConnectionsSubSet { get; private set; }


        public ObservableCollection<UserViewModel> CurrentUserIdentities { get; set; } = new ObservableCollection<UserViewModel>();

        public ICommand EditConnectionsCommand { get; set; }

        public BaseViewModel ProjectsSubSet { get; private set; }

        public event EventHandler? EditConnectionsRequested;

        public event EventHandler? SettingsChanged;

        public void UpdateUser()
        {
            var newUsers = Configuration.IdentitiesOfCurrentUser.Select(u => new UserViewModel(u)).ToList();

            var toAdd = newUsers.Where(nu => CurrentUserIdentities.All(cu => cu.User.Id != nu.User.Id)).ToList();
            var toRemove = CurrentUserIdentities.Where(cu => newUsers.All(nu => nu.User.Id != cu.User.Id)).ToList();

            foreach (var user in toAdd)
            {
                CurrentUserIdentities.Add(user);
            }

            foreach (var user in toRemove)
            {
                CurrentUserIdentities.Remove(user);
            }
        }


        private void OnConfigurationChanged(object? sender, EventArgs args)
        {
            _saveMethod.Invoke();
            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnEditConnections()
        {
            EditConnectionsRequested?.Invoke(this, EventArgs.Empty);
        }

        private readonly Action _saveMethod;
        private readonly IPluginRepository _pluginRepository;
    }
}