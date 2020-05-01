using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Text;
using BuildNotifications.ViewModel.Settings.Options;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        public SettingsViewModel(IConfiguration configuration, Action saveMethod, IUserIdentityList userIdentityList)
        {
            _saveMethod = saveMethod;
            _userIdentityList = userIdentityList;
            _configuration = configuration;
            EditConnectionsCommand = new DelegateCommand(EditConnections);

            Language = new LanguageOptionViewModel(configuration.Language);
            AnimationsMode = new EnumOptionViewModel<AnimationMode>(StringLocalizer.Keys.AnimationSpeed, configuration.AnimationSpeed);
            AutoStartMode = new EnumOptionViewModel<AutostartMode>(StringLocalizer.Keys.Autostart, configuration.AutoStart);
            CanceledBuildNotify = new EnumOptionViewModel<BuildNotificationModes>(StringLocalizer.Keys.CanceledBuildNotifyConfig, configuration.CanceledBuildNotifyConfig);
            FailedBuildNotify = new EnumOptionViewModel<BuildNotificationModes>(StringLocalizer.Keys.FailedBuildNotifyConfig, configuration.FailedBuildNotifyConfig);
            SucceededBuildNotify = new EnumOptionViewModel<BuildNotificationModes>(StringLocalizer.Keys.SucceededBuildNotifyConfig, configuration.SucceededBuildNotifyConfig);
            PartialSucceededTreatmentMode = new EnumOptionViewModel<PartialSucceededTreatmentMode>(StringLocalizer.Keys.PartialSucceededTreatmentMode, configuration.PartialSucceededTreatmentMode);
            BuildsPerGroup = new NumberOptionViewModel(configuration.BuildsToShow, 1, 100, StringLocalizer.Keys.BuildsToShow);
            ShowBusyIndicatorDuringUpdate = new BooleanOptionViewModel(configuration.ShowBusyIndicatorOnDeltaUpdates, StringLocalizer.Keys.ShowBusyIndicatorOnDeltaUpdates);
            UpdateInterval = new NumberOptionViewModel(configuration.UpdateInterval, 30, int.MaxValue, StringLocalizer.Keys.UpdateInterval);
            UpdateToPreReleases = new BooleanOptionViewModel(configuration.UsePreReleases, StringLocalizer.Keys.UsePreReleases);

            foreach (var option in Options)
            {
                option.ValueChanged += Option_ValueChanged;
            }

            UpdateUser();
        }

        public ObservableCollection<UserViewModel> CurrentUserIdentities { get; } = new ObservableCollection<UserViewModel>();

        public ICommand EditConnectionsCommand { get; }

        public IEnumerable<OptionViewModelBase> Options
        {
            get
            {
                yield return Language;
                yield return BuildsPerGroup;
                yield return UpdateInterval;
                yield return UpdateToPreReleases;
                yield return CanceledBuildNotify;
                yield return FailedBuildNotify;
                yield return SucceededBuildNotify;
                yield return PartialSucceededTreatmentMode;
                yield return AutoStartMode;
                yield return AnimationsMode;
                yield return ShowBusyIndicatorDuringUpdate;
            }
        }

        internal EnumOptionViewModel<AnimationMode> AnimationsMode { get; }
        internal EnumOptionViewModel<AutostartMode> AutoStartMode { get; }
        internal NumberOptionViewModel BuildsPerGroup { get; }
        internal EnumOptionViewModel<BuildNotificationModes> CanceledBuildNotify { get; }
        internal EnumOptionViewModel<BuildNotificationModes> FailedBuildNotify { get; }
        internal LanguageOptionViewModel Language { get; }
        internal EnumOptionViewModel<PartialSucceededTreatmentMode> PartialSucceededTreatmentMode { get; }
        internal BooleanOptionViewModel ShowBusyIndicatorDuringUpdate { get; }
        internal EnumOptionViewModel<BuildNotificationModes> SucceededBuildNotify { get; }
        internal NumberOptionViewModel UpdateInterval { get; }
        internal BooleanOptionViewModel UpdateToPreReleases { get; }

        public event EventHandler<EventArgs>? EditConnectionsRequested;

        public void UpdateUser()
        {
            var newUsers = _userIdentityList.IdentitiesOfCurrentUser.Select(u => new UserViewModel(u)).ToList();

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

        private void EditConnections()
        {
            EditConnectionsRequested?.Invoke(this, EventArgs.Empty);
        }

        private void Option_ValueChanged(object? sender, EventArgs e)
        {
            _configuration.AnimationSpeed = AnimationsMode.Value;
            _configuration.AutoStart = AutoStartMode.Value;
            _configuration.BuildsToShow = BuildsPerGroup.Value;
            _configuration.CanceledBuildNotifyConfig = CanceledBuildNotify.Value;
            _configuration.FailedBuildNotifyConfig = FailedBuildNotify.Value;
            _configuration.Language = Language.Value.IetfLanguageTag;
            _configuration.PartialSucceededTreatmentMode = PartialSucceededTreatmentMode.Value;
            _configuration.ShowBusyIndicatorOnDeltaUpdates = ShowBusyIndicatorDuringUpdate.Value;
            _configuration.SucceededBuildNotifyConfig = SucceededBuildNotify.Value;
            _configuration.UpdateInterval = UpdateInterval.Value;
            _configuration.UsePreReleases = UpdateToPreReleases.Value;

            _saveMethod.Invoke();
        }

        private readonly IConfiguration _configuration;

        private readonly Action _saveMethod;
        private readonly IUserIdentityList _userIdentityList;
    }
}