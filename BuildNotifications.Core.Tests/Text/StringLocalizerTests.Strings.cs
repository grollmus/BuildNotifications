﻿using System;
using Xunit;
using BuildNotifications.Core.Text;

// ReSharper disable All
namespace BuildNotifications.Core.Tests.Text
{
	public partial class StringLocalizerTests
	{
        public static TheoryData<string, Func<string>> TextProperties => new TheoryData<string, Func<string>>
        {
           {"NativeName", () => StringLocalizer.NativeName},
           {"GroupBy", () => StringLocalizer.GroupBy},
           {"ThenBy", () => StringLocalizer.ThenBy},
           {"Status", () => StringLocalizer.Status},
           {"BuildDefinition", () => StringLocalizer.BuildDefinition},
           {"Source", () => StringLocalizer.Source},
           {"None", () => StringLocalizer.None},
           {"Branch", () => StringLocalizer.Branch},
           {"StatusDescending", () => StringLocalizer.StatusDescending},
           {"StatusAscending", () => StringLocalizer.StatusAscending},
           {"AlphabeticalDescending", () => StringLocalizer.AlphabeticalDescending},
           {"AlphabeticalAscending", () => StringLocalizer.AlphabeticalAscending},
           {"Sort", () => StringLocalizer.Sort},
           {"BuildPluginType", () => StringLocalizer.BuildPluginType},
           {"SourceControlPluginType", () => StringLocalizer.SourceControlPluginType},
           {"Name", () => StringLocalizer.Name},
           {"Options", () => StringLocalizer.Options},
           {"ProjectName", () => StringLocalizer.ProjectName},
           {"BranchBlacklist", () => StringLocalizer.BranchBlacklist},
           {"BranchWhitelist", () => StringLocalizer.BranchWhitelist},
           {"BuildConnectionNames", () => StringLocalizer.BuildConnectionNames},
           {"Connections", () => StringLocalizer.Connections},
           {"BuildDefinitionBlacklist", () => StringLocalizer.BuildDefinitionBlacklist},
           {"BuildDefinitionWhitelist", () => StringLocalizer.BuildDefinitionWhitelist},
           {"DefaultCompareBranch", () => StringLocalizer.DefaultCompareBranch},
           {"HideCompletedPullRequests", () => StringLocalizer.HideCompletedPullRequests},
           {"LoadWhitelistedBranchesExclusively", () => StringLocalizer.LoadWhitelistedBranchesExclusively},
           {"LoadWhitelistedDefinitionsExclusively", () => StringLocalizer.LoadWhitelistedDefinitionsExclusively},
           {"ShowPullRequests", () => StringLocalizer.ShowPullRequests},
           {"SourceControlConnectionNames", () => StringLocalizer.SourceControlConnectionNames},
           {"NewConnection", () => StringLocalizer.NewConnection},
           {"NewProject", () => StringLocalizer.NewProject},
           {"Projects", () => StringLocalizer.Projects},
           {"BuildsToLoadCount", () => StringLocalizer.BuildsToLoadCount},
           {"BuildsToShow", () => StringLocalizer.BuildsToShow},
           {"UpdateInterval", () => StringLocalizer.UpdateInterval},
           {"CanceledBuildNotifyConfig", () => StringLocalizer.CanceledBuildNotifyConfig},
           {"FailedBuildNotifyConfig", () => StringLocalizer.FailedBuildNotifyConfig},
           {"SucceededBuildNotifyConfig", () => StringLocalizer.SucceededBuildNotifyConfig},
           {"Language", () => StringLocalizer.Language},
           {"InitialSetupEmptyConf", () => StringLocalizer.InitialSetupEmptyConf},
           {"InitialSetupEmptyConnections", () => StringLocalizer.InitialSetupEmptyConnections},
           {"InitialSetupConnectionNotAsBuildOrSource", () => StringLocalizer.InitialSetupConnectionNotAsBuildOrSource},
           {"ErrorFetchingBranches", () => StringLocalizer.ErrorFetchingBranches},
           {"ErrorFetchingBuilds", () => StringLocalizer.ErrorFetchingBuilds},
           {"ErrorFetchingDefinitions", () => StringLocalizer.ErrorFetchingDefinitions},
           {"DefinitionChangedTextId", () => StringLocalizer.DefinitionChangedTextId},
           {"TwoDefinitionsChangedTextId", () => StringLocalizer.TwoDefinitionsChangedTextId},
           {"ThreeDefinitionsChangedTextId", () => StringLocalizer.ThreeDefinitionsChangedTextId},
           {"BuildChangedTextId", () => StringLocalizer.BuildChangedTextId},
           {"BuildsChangedTextId", () => StringLocalizer.BuildsChangedTextId},
           {"FailedSingular", () => StringLocalizer.FailedSingular},
           {"FailedPlural", () => StringLocalizer.FailedPlural},
           {"SucceededSingular", () => StringLocalizer.SucceededSingular},
           {"SucceededPlural", () => StringLocalizer.SucceededPlural},
           {"CancelledSingular", () => StringLocalizer.CancelledSingular},
           {"CancelledPlural", () => StringLocalizer.CancelledPlural},
           {"BranchChangedTextId", () => StringLocalizer.BranchChangedTextId},
           {"TwoBranchesChangedTextId", () => StringLocalizer.TwoBranchesChangedTextId},
           {"ThreeBranchesChangedTextId", () => StringLocalizer.ThreeBranchesChangedTextId},
           {"BranchAndDefinitionFailedTextId", () => StringLocalizer.BranchAndDefinitionFailedTextId},
           {"BuildNotificationContentPluralTextId", () => StringLocalizer.BuildNotificationContentPluralTextId},
           {"BuildNotificationContentSingularTextId", () => StringLocalizer.BuildNotificationContentSingularTextId},
           {"NDaysAgoTextId", () => StringLocalizer.NDaysAgoTextId},
           {"OneDayAgoTextId", () => StringLocalizer.OneDayAgoTextId},
           {"NHoursAgoTextId", () => StringLocalizer.NHoursAgoTextId},
           {"NMinutesAgoTextId", () => StringLocalizer.NMinutesAgoTextId},
           {"NSecondsAgoTextId", () => StringLocalizer.NSecondsAgoTextId},
           {"JustNowTextId", () => StringLocalizer.JustNowTextId},
           {"AnErrorOccured", () => StringLocalizer.AnErrorOccured},
           {"ConnectionNotFound", () => StringLocalizer.ConnectionNotFound},
           {"SourceControlPluginNotFound", () => StringLocalizer.SourceControlPluginNotFound},
           {"FailedToConstructBranchProviderFromPlugin", () => StringLocalizer.FailedToConstructBranchProviderFromPlugin},
           {"BuildPluginNotFound", () => StringLocalizer.BuildPluginNotFound},
           {"FailedToConstructBuildProviderFromPlugin", () => StringLocalizer.FailedToConstructBuildProviderFromPlugin},
           {"FailedToConstructBuildProviderForConnection", () => StringLocalizer.FailedToConstructBuildProviderForConnection},
           {"FailedToConstructBranchProviderForConnection", () => StringLocalizer.FailedToConstructBranchProviderForConnection},
           {"DateDescending", () => StringLocalizer.DateDescending},
           {"ErrorFetchingUserIdentities", () => StringLocalizer.ErrorFetchingUserIdentities},
           {"EditConnection", () => StringLocalizer.EditConnection},
           {"TestConnection", () => StringLocalizer.TestConnection},
           {"TreeIsEmpty", () => StringLocalizer.TreeIsEmpty},
           {"Testing", () => StringLocalizer.Testing},
           {"NoNotifications", () => StringLocalizer.NoNotifications},
           {"BuildConnectionTestFailed", () => StringLocalizer.BuildConnectionTestFailed},
           {"SourceConnectionTestFailed", () => StringLocalizer.SourceConnectionTestFailed},
           {"ConnectionTestSuccessful", () => StringLocalizer.ConnectionTestSuccessful},
           {"ConnectionTestCaption", () => StringLocalizer.ConnectionTestCaption},
           {"InitialSetupUntested", () => StringLocalizer.InitialSetupUntested},
           {"InitialSetupTested", () => StringLocalizer.InitialSetupTested},
           {"InitialSetupConnectionNotAsBuild", () => StringLocalizer.InitialSetupConnectionNotAsBuild},
           {"InitialSetupConnectionNotAsSource", () => StringLocalizer.InitialSetupConnectionNotAsSource},
           {"CloseConnections", () => StringLocalizer.CloseConnections},
           {"InitialSetupCompleteConfig", () => StringLocalizer.InitialSetupCompleteConfig},
           {"NotificationNotFound", () => StringLocalizer.NotificationNotFound},
           {"BranchesCount", () => StringLocalizer.BranchesCount},
           {"DefinitionsCount", () => StringLocalizer.DefinitionsCount},
           {"ShowWindow", () => StringLocalizer.ShowWindow},
           {"Exit", () => StringLocalizer.Exit},
           {"GoToBuild", () => StringLocalizer.GoToBuild},
           {"GoToBranch", () => StringLocalizer.GoToBranch},
           {"GoToDefinition", () => StringLocalizer.GoToDefinition},
           {"Pending", () => StringLocalizer.Pending},
           {"Running", () => StringLocalizer.Running},
           {"Failed", () => StringLocalizer.Failed},
           {"Succeeded", () => StringLocalizer.Succeeded},
           {"Cancelled", () => StringLocalizer.Cancelled},
           {"PartiallySucceeded", () => StringLocalizer.PartiallySucceeded},
           {"RequestedBy", () => StringLocalizer.RequestedBy},
           {"RequestedFor", () => StringLocalizer.RequestedFor},
           {"IsEnabled", () => StringLocalizer.IsEnabled},
           {"Never", () => StringLocalizer.Never},
           {"Always", () => StringLocalizer.Always},
           {"RequestedForMe", () => StringLocalizer.RequestedForMe},
           {"RequestedByOrForMe", () => StringLocalizer.RequestedByOrForMe},
           {"RequestedByMe", () => StringLocalizer.RequestedByMe},
           {"YourIdentities", () => StringLocalizer.YourIdentities},
           {"UserDescription", () => StringLocalizer.UserDescription},
           {"ClearAll", () => StringLocalizer.ClearAll},
           {"OneOrMoreErrors", () => StringLocalizer.OneOrMoreErrors},
           {"BuildsPaused", () => StringLocalizer.BuildsPaused},
           {"Autostart", () => StringLocalizer.Autostart},
           {"DontAutostart", () => StringLocalizer.DontAutostart},
           {"StartWithWindows", () => StringLocalizer.StartWithWindows},
           {"StartWithWindowsMinimized", () => StringLocalizer.StartWithWindowsMinimized},
           {"PartialSucceededTreatmentMode", () => StringLocalizer.PartialSucceededTreatmentMode},
           {"TreatAsSucceeded", () => StringLocalizer.TreatAsSucceeded},
           {"Ignore", () => StringLocalizer.Ignore},
           {"TreatAsFailed", () => StringLocalizer.TreatAsFailed},
           {"UsePreReleases", () => StringLocalizer.UsePreReleases},
           {"DoubleSpeed", () => StringLocalizer.DoubleSpeed},
           {"AnimationSpeed", () => StringLocalizer.AnimationSpeed},
           {"Enabled", () => StringLocalizer.Enabled},
           {"Disabled", () => StringLocalizer.Disabled},
           {"ShowBusyIndicatorOnDeltaUpdates", () => StringLocalizer.ShowBusyIndicatorOnDeltaUpdates},
           {"NoValue", () => StringLocalizer.NoValue},
           {"OneAndHalfSpeed", () => StringLocalizer.OneAndHalfSpeed},
           {"ThreeQuartersSpeed", () => StringLocalizer.ThreeQuartersSpeed},
           {"HalfSpeed", () => StringLocalizer.HalfSpeed},
           {"OneAndQuarterSpeed", () => StringLocalizer.OneAndQuarterSpeed},
           {"CheckForUpdates", () => StringLocalizer.CheckForUpdates},
           {"NoUpdatesAvailable", () => StringLocalizer.NoUpdatesAvailable},
           {"UpdatesAvailable", () => StringLocalizer.UpdatesAvailable},
           {"CheckingForUpdates", () => StringLocalizer.CheckingForUpdates},
           {"AnUpdateHasBeenInstalled", () => StringLocalizer.AnUpdateHasBeenInstalled},
           {"RestartApplicationToApplyUpdate", () => StringLocalizer.RestartApplicationToApplyUpdate},
           {"GoToPullRequest", () => StringLocalizer.GoToPullRequest},
           {"InfoButtonTooltip", () => StringLocalizer.InfoButtonTooltip},
           {"NotificationsCenterButtonTooltip", () => StringLocalizer.NotificationsCenterButtonTooltip},
           {"SettingsButtonTooltip", () => StringLocalizer.SettingsButtonTooltip},
           {"GroupDefinitionsTooltip", () => StringLocalizer.GroupDefinitionsTooltip},
           {"CloseButtonTooltip", () => StringLocalizer.CloseButtonTooltip},
           {"MaximizeButtonTooltip", () => StringLocalizer.MaximizeButtonTooltip},
           {"MinimizeButtonTooltip", () => StringLocalizer.MinimizeButtonTooltip},
           {"RestoreButtonTooltip", () => StringLocalizer.RestoreButtonTooltip},
           {"SearchTooltip", () => StringLocalizer.SearchTooltip},
           {"ConnectionType", () => StringLocalizer.ConnectionType},
           {"ConnectionNameDescription", () => StringLocalizer.ConnectionNameDescription},
           {"ConnectionTypeDescription", () => StringLocalizer.ConnectionTypeDescription},
           {"ConfirmDeletion", () => StringLocalizer.ConfirmDeletion},
           {"ConfirmDeleteConnection", () => StringLocalizer.ConfirmDeleteConnection},
           {"ConfirmDeleteProject", () => StringLocalizer.ConfirmDeleteProject},
           {"Yes", () => StringLocalizer.Yes},
           {"No", () => StringLocalizer.No},
           {"Ok", () => StringLocalizer.Ok},
           {"Cancel", () => StringLocalizer.Cancel},
           {"UseDarkTheme", () => StringLocalizer.UseDarkTheme},
           {"MyBuildsSightToolTip", () => StringLocalizer.MyBuildsSightToolTip},
           {"ManualBuildsSightToolTip", () => StringLocalizer.ManualBuildsSightToolTip},
           {"SearchDefaultDescription", () => StringLocalizer.SearchDefaultDescription},
           {"SearchCriteriaAfterKeyword", () => StringLocalizer.SearchCriteriaAfterKeyword},
           {"SearchCriteriaAfterDescription", () => StringLocalizer.SearchCriteriaAfterDescription},
           {"SearchCriteriaAfterYesterday", () => StringLocalizer.SearchCriteriaAfterYesterday},
           {"SearchCriteriaBeforeKeyword", () => StringLocalizer.SearchCriteriaBeforeKeyword},
           {"SearchCriteriaBeforeDescription", () => StringLocalizer.SearchCriteriaBeforeDescription},
           {"SearchCriteriaBeforeToday", () => StringLocalizer.SearchCriteriaBeforeToday},
           {"SearchCriteriaDuringKeyword", () => StringLocalizer.SearchCriteriaDuringKeyword},
           {"SearchCriteriaDuringDescription", () => StringLocalizer.SearchCriteriaDuringDescription},
           {"SearchCriteriaDuringToday", () => StringLocalizer.SearchCriteriaDuringToday},
           {"SearchCriteriaByKeyword", () => StringLocalizer.SearchCriteriaByKeyword},
           {"SearchCriteriaByDescription", () => StringLocalizer.SearchCriteriaByDescription},
           {"SearchCriteriaForKeyword", () => StringLocalizer.SearchCriteriaForKeyword},
           {"SearchCriteriaForDescription", () => StringLocalizer.SearchCriteriaForDescription},
           {"SearchCriteriaIsKeyword", () => StringLocalizer.SearchCriteriaIsKeyword},
           {"SearchCriteriaIsDescription", () => StringLocalizer.SearchCriteriaIsDescription},
           {"SearchCriteriaBranchKeyword", () => StringLocalizer.SearchCriteriaBranchKeyword},
           {"SearchCriteriaBranchDescription", () => StringLocalizer.SearchCriteriaBranchDescription},
           {"SearchCriteriaDefinitionKeyword", () => StringLocalizer.SearchCriteriaDefinitionKeyword},
           {"SearchCriteriaDefinitionDescription", () => StringLocalizer.SearchCriteriaDefinitionDescription},
           {"SearchCriteriaDuringYesterday", () => StringLocalizer.SearchCriteriaDuringYesterday},
           {"SearchCriteriaIsManual", () => StringLocalizer.SearchCriteriaIsManual},
           {"SearchCriteriaIsByMe", () => StringLocalizer.SearchCriteriaIsByMe},
           {"SearchCriteriaIsInProgress", () => StringLocalizer.SearchCriteriaIsInProgress},
           {"SearchCriteriaIsPending", () => StringLocalizer.SearchCriteriaIsPending},
           {"SearchCriteriaIsFailed", () => StringLocalizer.SearchCriteriaIsFailed},
           {"SearchCriteriaIsCancelled", () => StringLocalizer.SearchCriteriaIsCancelled},
           {"SearchCriteriaIsCi", () => StringLocalizer.SearchCriteriaIsCi},
           {"SearchCriteriaIsNightly", () => StringLocalizer.SearchCriteriaIsNightly},
           {"SearchCriteriaIsPullRequest", () => StringLocalizer.SearchCriteriaIsPullRequest},
           {"SearchCriteriaIsSucceeded", () => StringLocalizer.SearchCriteriaIsSucceeded},
           {"SearchCriteriaIsPartiallySucceeded", () => StringLocalizer.SearchCriteriaIsPartiallySucceeded},
           {"SearchCriteriaIsScheduled", () => StringLocalizer.SearchCriteriaIsScheduled},
           {"SearchCriteriaBranchStageExample", () => StringLocalizer.SearchCriteriaBranchStageExample},
           {"SearchCriteriaDefinitionNightlyExample", () => StringLocalizer.SearchCriteriaDefinitionNightlyExample},
           {"SearchCriteriaForMeExample", () => StringLocalizer.SearchCriteriaForMeExample},
           {"SearchCriteriaByMeExample", () => StringLocalizer.SearchCriteriaByMeExample},
           {"SearchCriteriaForSomeoneExample", () => StringLocalizer.SearchCriteriaForSomeoneExample},
           {"SearchCriteriaBySomeoneExample", () => StringLocalizer.SearchCriteriaBySomeoneExample},
        };
	}
}