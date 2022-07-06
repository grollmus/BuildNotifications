using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Text;
using BuildNotifications.Services;
using BuildNotifications.ViewModel.Settings.Options;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel.Settings;

internal class SettingsViewModel : BaseViewModel
{
    public SettingsViewModel(IConfigurationService configurationService, Action saveMethod, IUserIdentityList userIdentityList, IPopupService popupService)
    {
        _saveMethod = saveMethod;
        _userIdentityList = userIdentityList;
        _popupService = popupService;
        _configurationService = configurationService;
        EditConnectionsCommand = new DelegateCommand(EditConnections);
        ImportExportCommand = new DelegateCommand(ImportExport);

        var configuration = _configurationService.Current;
        Language = new LanguageOptionViewModel(configuration.Language);
        AnimationsMode = new EnumOptionViewModel<AnimationMode>(StringLocalizer.Keys.AnimationSpeed, configuration.AnimationSpeed);
        AutoStartMode = new EnumOptionViewModel<AutostartMode>(StringLocalizer.Keys.Autostart, configuration.Autostart);
        CanceledBuildNotify = new EnumOptionViewModel<BuildNotificationModes>(StringLocalizer.Keys.CanceledBuildNotifyConfig, configuration.CanceledBuildNotifyConfig);
        FailedBuildNotify = new EnumOptionViewModel<BuildNotificationModes>(StringLocalizer.Keys.FailedBuildNotifyConfig, configuration.FailedBuildNotifyConfig);
        SucceededBuildNotify = new EnumOptionViewModel<BuildNotificationModes>(StringLocalizer.Keys.SucceededBuildNotifyConfig, configuration.SucceededBuildNotifyConfig);
        PartialSucceededTreatmentMode = new EnumOptionViewModel<PartialSucceededTreatmentMode>(StringLocalizer.Keys.PartialSucceededTreatmentMode, configuration.PartialSucceededTreatmentMode);
        BuildsPerGroup = new NumberOptionViewModel(configuration.BuildsToShow, 1, 100, StringLocalizer.Keys.BuildsToShow);
        ShowBusyIndicatorDuringUpdate = new BooleanOptionViewModel(configuration.ShowBusyIndicatorOnDeltaUpdates, StringLocalizer.Keys.ShowBusyIndicatorOnDeltaUpdates);
        UpdateInterval = new NumberOptionViewModel(configuration.UpdateInterval, 30, int.MaxValue, StringLocalizer.Keys.UpdateInterval);
        UpdateToPreReleases = new BooleanOptionViewModel(configuration.UsePreReleases, StringLocalizer.Keys.UsePreReleases);
        UseDarkTheme = new BooleanOptionViewModel(configuration.ApplicationTheme == Theme.Dark, StringLocalizer.Keys.UseDarkTheme);

        foreach (var option in Options)
        {
            option.ValueChanged += Option_ValueChanged;
        }

        UpdateUser();
    }

    public ObservableCollection<UserViewModel> CurrentUserIdentities { get; } = new();

    public ICommand EditConnectionsCommand { get; }
    public ICommand ImportExportCommand { get; }

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
            yield return UseDarkTheme;
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
    internal BooleanOptionViewModel UseDarkTheme { get; }

    public event EventHandler<EventArgs>? EditConnectionsRequested;
    public event EventHandler<EventArgs>? ReloadRequested;

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

    private void ImportExport()
    {
        _popupService.ShowImportExportPopup(_configurationService);
        UpdateOptions();
        ReloadRequested?.Invoke(this, EventArgs.Empty);
    }

    private void Option_ValueChanged(object? sender, EventArgs e)
    {
        _configurationService.Current.AnimationSpeed = AnimationsMode.Value;
        _configurationService.Current.Autostart = AutoStartMode.Value;
        _configurationService.Current.BuildsToShow = BuildsPerGroup.Value;
        _configurationService.Current.CanceledBuildNotifyConfig = CanceledBuildNotify.Value;
        _configurationService.Current.FailedBuildNotifyConfig = FailedBuildNotify.Value;
        _configurationService.Current.Language = Language.Value.IetfLanguageTag;
        _configurationService.Current.PartialSucceededTreatmentMode = PartialSucceededTreatmentMode.Value;
        _configurationService.Current.ShowBusyIndicatorOnDeltaUpdates = ShowBusyIndicatorDuringUpdate.Value;
        _configurationService.Current.SucceededBuildNotifyConfig = SucceededBuildNotify.Value;
        _configurationService.Current.UpdateInterval = UpdateInterval.Value;
        _configurationService.Current.UsePreReleases = UpdateToPreReleases.Value;
        _configurationService.Current.ApplicationTheme = UseDarkTheme.Value ? Theme.Dark : Theme.Light;

        _saveMethod.Invoke();
    }

    private void UpdateOptions()
    {
        Language.Value = new CultureInfo(_configurationService.Current.Language);
        AnimationsMode.Value = _configurationService.Current.AnimationSpeed;
        AutoStartMode.Value = _configurationService.Current.Autostart;
        CanceledBuildNotify.Value = _configurationService.Current.CanceledBuildNotifyConfig;
        FailedBuildNotify.Value = _configurationService.Current.FailedBuildNotifyConfig;
        SucceededBuildNotify.Value = _configurationService.Current.SucceededBuildNotifyConfig;
        PartialSucceededTreatmentMode.Value = _configurationService.Current.PartialSucceededTreatmentMode;
        BuildsPerGroup.Value = _configurationService.Current.BuildsToShow;
        ShowBusyIndicatorDuringUpdate.Value = _configurationService.Current.ShowBusyIndicatorOnDeltaUpdates;
        UpdateInterval.Value = _configurationService.Current.UpdateInterval;
        UpdateToPreReleases.Value = _configurationService.Current.UsePreReleases;
        UseDarkTheme.Value = _configurationService.Current.ApplicationTheme == Theme.Dark;
    }

    private readonly IConfigurationService _configurationService;

    private readonly Action _saveMethod;
    private readonly IUserIdentityList _userIdentityList;
    private readonly IPopupService _popupService;
}