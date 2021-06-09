using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BuildNotifications.Core;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Core.Pipeline.Notification.Distribution;
using BuildNotifications.Core.Protocol;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.Services;
using BuildNotifications.ViewModel.GroupDefinitionSelection;
using BuildNotifications.ViewModel.Notification;
using BuildNotifications.ViewModel.Overlays;
using BuildNotifications.ViewModel.Settings;
using BuildNotifications.ViewModel.Tree;
using BuildNotifications.ViewModel.Utils;
using BuildNotifications.ViewModel.Utils.Configuration;
using NLog.Fluent;
using Polly;
using Polly.CircuitBreaker;
using Semver;
using TweenSharp.Animation;
using TweenSharp.Factory;

namespace BuildNotifications.ViewModel
{
	internal class MainViewModel : BaseViewModel, IDisposable, IBlurrableViewModel
	{
// properties *are* initialized within the constructor. However by a method call, which is not correctly recognized by the code analyzer yet.
#pragma warning disable CS8618 // warning about uninitialized non-nullable properties
		public MainViewModel(IViewProvider viewProvider)
#pragma warning restore CS8618
		{
			var pathResolver = new PathResolver();
			_fileWatch = new FileWatchDistributedNotificationReceiver(pathResolver);
			_trayIcon = new TrayIconHandle();
			_trayIcon.ExitRequested += TrayIconOnExitRequested;
			_trayIcon.ShowWindowRequested += TrayIconOnShowWindowRequested;
			var dispatcher = new WpfDispatcher();
			_coreSetup = new CoreSetup(pathResolver, _fileWatch, dispatcher);
			_coreSetup.PipelineUpdated += CoreSetup_PipelineUpdated;
			_coreSetup.DistributedNotificationReceived += CoreSetup_DistributedNotificationReceived;
			_configurationApplication = new ConfigurationApplication(_coreSetup.Configuration);
			_configurationApplication.ApplyChanges();
			GlobalErrorLogTarget.ErrorOccured += GlobalErrorLog_ErrorOccurred;
			_popupService = new PopupService(this, viewProvider);
			_windowSettings = new WindowSettings(pathResolver.WindowSettingsFilePath);
			_updateUrls = new UpdateUrls();
			Initialize();
		}

		public bool BlurView
		{
			get => _blurView;
			set
			{
				_blurView = value;
				OnPropertyChanged();
			}
		}

		public BuildTreeViewModel? BuildTree
		{
			get => _buildTree;
			set
			{
				_buildTree = value;
				OnPropertyChanged();
			}
		}

		public GroupAndSortDefinitionsViewModel GroupAndSortDefinitionsSelection { get; set; }

		public NotificationCenterViewModel NotificationCenter { get; set; }

		public BaseViewModel? Overlay
		{
			get => _overlay;
			set
			{
				_overlay = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(TitleBarToolsVisibility));
			}
		}

		public SearchViewModel SearchViewModel { get; set; }

		public SettingsViewModel SettingsViewModel { get; set; }

		public bool ShowGroupDefinitionSelection
		{
			get => _showGroupDefinitionSelection;
			set
			{
				_showGroupDefinitionSelection = value;
				OnPropertyChanged();
			}
		}

		public ICommand ShowInfoPopupCommand { get; set; }

		public bool ShowNotificationCenter
		{
			get => _showNotificationCenter;
			set
			{
				_showNotificationCenter = value;
				OnPropertyChanged();
			}
		}

		public bool ShowSettings
		{
			get => _showSettings;
			set
			{
				_showSettings = value;
				OnPropertyChanged();
			}
		}

		public StatusIndicatorViewModel StatusIndicator { get; set; }

		public Visibility TitleBarToolsVisibility => Overlay == null ? Visibility.Visible : Visibility.Collapsed;

		public ICommand ToggleGroupDefinitionSelectionCommand { get; set; }
		public ICommand ToggleShowNotificationCenterCommand { get; set; }
		public ICommand ToggleShowSettingsCommand { get; set; }

		public void RestoreWindowStateFor(Window window)
		{
			_windowSettings.ApplyTo(window);
		}

		public void SaveWindowStateOf(Window window)
		{
			_windowSettings.Save(window);
		}

		private void BringWindowToFront()
		{
			var mainWindow = Application.Current.MainWindow;
			if (mainWindow != null)
			{
				if (mainWindow.WindowState == WindowState.Minimized)
					mainWindow.WindowState = WindowState.Normal;

				mainWindow.Visibility = Visibility.Visible;
				mainWindow.Activate();
				mainWindow.Show();
				mainWindow.Topmost = true;
				mainWindow.Topmost = false;
				mainWindow.Focus();
			}
		}

		private void CoreSetup_DistributedNotificationReceived(object? sender, DistributedNotificationReceivedEventArgs e)
		{
			Application.Current.Dispatcher?.Invoke(() =>
			{
				BringWindowToFront();

				if (e.DistributedNotification.BasedOnNotification != null)
				{
					var success = NotificationCenter.TryHighlightNotificationByGuid(e.DistributedNotification.BasedOnNotification.Value);
					if (!success)
						ShowNotifications(new List<INotification> {new StatusNotification(e.DistributedNotification.BasedOnNotification.Value.ToString(), StringLocalizer.NotificationNotFound, NotificationType.Info)});

					if (!ShowNotificationCenter)
						ToggleShowNotificationCenter();
				}
			});
		}

		private void CoreSetup_PipelineUpdated(object? sender, PipelineUpdateEventArgs e)
		{
			_postPipelineUpdateTask = UpdateTreeTask(e);
			SettingsViewModel.UpdateUser();
		}

		private void GlobalErrorLog_ErrorOccurred(object? sender, ErrorNotificationEventArgs e)
		{
			StatusIndicator.Error(e.ErrorNotifications);

			// errors may occur on any thread.
			Application.Current.Dispatcher?.Invoke(() => { ShowNotifications(e.ErrorNotifications); });
		}

		private void GroupAndSortDefinitionsSelectionOnPropertyChanged(object? sender, PropertyChangedEventArgs args)
		{
			switch (args.PropertyName)
			{
				case nameof(GroupAndSortDefinitionsViewModel.BuildTreeGroupDefinition):
					_coreSetup.Configuration.GroupDefinition = GroupAndSortDefinitionsSelection.BuildTreeGroupDefinition;
					_coreSetup.PersistConfigurationChanges();
					BuildTree = null;
					UpdateNow();
					break;
				case nameof(GroupAndSortDefinitionsViewModel.BuildTreeSortingDefinition) when BuildTree != null:
					_coreSetup.Configuration.SortingDefinition = GroupAndSortDefinitionsSelection.BuildTreeSortingDefinition;
					_coreSetup.PersistConfigurationChanges();
					BuildTree.SortingDefinition = GroupAndSortDefinitionsSelection.BuildTreeSortingDefinition;
					break;
			}
		}

		private async Task HandleExistingDistributedNotificationsOnNextFrame()
		{
			// wait until next frame is rendered
			await WaitUntilNextFrameIsRenderedAsync();
			// remove any image files that may still exist from last launch
			NotificationDistributor.DeleteAllTemporaryImageFiles();
			// set initial tray icon (do this here, so any instance which immediately exits doesn't spawn an icon)
			_trayIcon.BuildStatus = BuildStatus.None;
			_fileWatch.HandleAllExistingFiles();
		}

		private void Initialize()
		{
			SetupViewModel();
			LoadProjects();
			ShowOverlay();
			RegisterUriProtocol();
			HandleExistingDistributedNotificationsOnNextFrame().IgnoreResult();
			UpdateApp().IgnoreResult();

			if (Overlay == null)
				StartUpdating();
		}

		private void InitialSetup_CloseRequested(object? sender, InitialSetupEventArgs e)
		{
			if (sender is not InitialSetupOverlayViewModel vm)
				return;

			vm.CloseRequested -= InitialSetup_CloseRequested;
			var tween = vm.Tween(x => x.Opacity).To(0).In(0.5).Ease(Easing.ExpoEaseOut).OnComplete((_, _) =>
			{
				Overlay = null;

				// when connections or projects changed or the update is stopped. Now is the time to reload and restart the pipeline
				// as the user either changed or checked the critical settings
				if (e.ProjectOrConnectionsChanged)
					ResetAndRestart();

				StartUpdating();
			});

			App.GlobalTweenHandler.Add(tween);
		}

		private void LoadProjects()
		{
			_coreSetup.Pipeline.ClearProjects();
			var projectProvider = _coreSetup.ProjectProvider;
			foreach (var project in projectProvider.EnabledProjects())
			{
				_coreSetup.Pipeline.AddProject(project);
				_hasAnyProjects = true;
			}
		}

		private void NotificationCenterOnCloseRequested(object? sender, EventArgs e)
		{
			if (ShowNotificationCenter)
				ToggleShowNotificationCenter();

			if (NotificationCenter.NoNotifications)
			{
				StatusIndicator.ClearStatus();
				StatusIndicator.Resume();
			}
		}

		private void NotificationCenterOnHighlightRequested(object? sender, HighlightRequestedEventArgs e)
		{
			foreach (var buildNode in _highlightedBuilds)
			{
				buildNode.IsHighlighted = false;
			}

			if (BuildTree == null)
				return;

			var buildsVm = BuildTree.AllBuilds().Where(b => e.BuildNodes.Any(bn => b.Node.Build.Id == bn.Build.Id && b.Node.Build.ProjectName == bn.Build.ProjectName));
			foreach (var buildNode in buildsVm)
			{
				buildNode.IsHighlighted = true;
				_highlightedBuilds.Add(buildNode);
			}
		}

		private void PersistChanges()
		{
			_coreSetup.PersistConfigurationChanges();
			_configurationApplication.ApplyChanges();
		}

		private void RegisterUriProtocol()
		{
			UriSchemeRegistration.Register();
		}

		private void ResetAndRestart()
		{
			ResetError();
			StopUpdating();
			LoadProjects();
			StartUpdating();
		}

		private void ResetError()
		{
			if (StatusIndicator.ErrorVisible)
				StatusIndicator.ClearStatus();
		}

		private void SettingsViewModelOnEditConnectionsRequested(object? sender, EventArgs e)
		{
			ToggleShowSettingsCommand.Execute(null);
			ShowInitialSetupOverlayViewModel();
		}

		private void SettingsViewModelOnReloadRequested(object? sender, EventArgs e)
		{
			ResetAndRestart();
		}

		private void SetupNotificationCenter()
		{
			NotificationCenter = new NotificationCenterViewModel
			{
				ShowClearButton = true
			};

			foreach (var processor in _coreSetup.PluginRepository.NotificationProcessors)
			{
				NotificationCenter.NotificationDistributor.Add(processor);
			}

			NotificationCenter.NotificationDistributor.Add(_trayIcon);
			NotificationCenter.HighlightRequested += NotificationCenterOnHighlightRequested;
			NotificationCenter.CloseRequested += NotificationCenterOnCloseRequested;
		}

		private void SetupViewModel()
		{
			SearchViewModel = new SearchViewModel(_coreSetup.Pipeline, _coreSetup.SearchEngine, _coreSetup.SearchHistory);
			StatusIndicator = new StatusIndicatorViewModel();
			StatusIndicator.ResumeRequested += StatusIndicator_OnResumeRequested;
			StatusIndicator.OpenErrorMessageRequested += StatusIndicator_OnOpenErrorMessageRequested;

			SetupNotificationCenter();

			SettingsViewModel = new SettingsViewModel(_coreSetup.ConfigurationService, PersistChanges, _coreSetup.UserIdentityList, _popupService);
			SettingsViewModel.EditConnectionsRequested += SettingsViewModelOnEditConnectionsRequested;
			SettingsViewModel.ReloadRequested += SettingsViewModelOnReloadRequested;

			GroupAndSortDefinitionsSelection = new GroupAndSortDefinitionsViewModel
			{
				BuildTreeGroupDefinition = _coreSetup.Configuration.GroupDefinition,
				BuildTreeSortingDefinition = _coreSetup.Configuration.SortingDefinition
			};
			GroupAndSortDefinitionsSelection.PropertyChanged += GroupAndSortDefinitionsSelectionOnPropertyChanged;

			ToggleGroupDefinitionSelectionCommand = new DelegateCommand(ToggleGroupDefinitionSelection);
			ToggleShowSettingsCommand = new DelegateCommand(ToggleShowSettings);
			ToggleShowNotificationCenterCommand = new DelegateCommand(ToggleShowNotificationCenter);
			ShowInfoPopupCommand = new DelegateCommand(ShowInfoPopup);
		}

		private void ShowInfoPopup()
		{
			var includePreReleases = _coreSetup.Configuration.UsePreReleases;
			var appUpdater = new AppUpdater(includePreReleases, NotificationCenter, _updateUrls);
			_popupService.ShowInfoPopup(includePreReleases, appUpdater);
		}

		private void ShowInitialSetupOverlayViewModel()
		{
			if (Overlay != null)
				return;

			StopUpdating();
			var vm = new InitialSetupOverlayViewModel(_coreSetup.Configuration, _coreSetup.PluginRepository, _coreSetup.ConfigurationBuilder, PersistChanges, _popupService);
			vm.CloseRequested += InitialSetup_CloseRequested;

			Overlay = vm;
		}

		private void ShowNotifications(IEnumerable<INotification> notifications)
		{
			NotificationCenter.ShowNotifications(notifications);
			if (ShowNotificationCenter)
				NotificationCenter.AllRead();
		}

		private void ShowOverlay()
		{
			var nothingConfigured = !_coreSetup.Configuration.Projects.Any() || !_coreSetup.Configuration.Connections.Any();
			if (!nothingConfigured)
				return;

			ShowInitialSetupOverlayViewModel();
		}

		private void StartUpdating()
		{
			if (!_hasAnyProjects)
			{
				Log.Info().Message("Not starting to update, as no projects are loaded.").Write();
				return;
			}

			Log.Info().Message("Start updating").Write();
			_keepUpdating = true;
			StatusIndicator.Resume();
			UpdateTimer().IgnoreResult();
			_fileWatch.Start();
		}

		private void StatusIndicator_OnOpenErrorMessageRequested(object? sender, OpenErrorRequestEventArgs e)
		{
			ToggleShowNotificationCenterCommand.Execute(null);
		}

		private void StatusIndicator_OnResumeRequested(object? sender, EventArgs e)
		{
			ResetAndRestart();
		}

		private void StopUpdating()
		{
			Log.Info().Message("Stop updating").Write();
			_isInitialFetch = true;
			_keepUpdating = false;
			UpdateNow(); // cancels the wait timer
			_fileWatch.Stop();
			StatusIndicator.Pause();
		}

		private void ToggleGroupDefinitionSelection()
		{
			Log.Info().Message($"Toggling group definition selection. Value: {!ShowGroupDefinitionSelection}").Write();
			ShowGroupDefinitionSelection = !ShowGroupDefinitionSelection;
		}

		private void ToggleShowNotificationCenter()
		{
			Log.Info().Message($"Toggling notification center. Value: {!ShowNotificationCenter}").Write();
			ShowNotificationCenter = !ShowNotificationCenter;
			if (ShowSettings && ShowNotificationCenter)
				ShowSettings = false;

			if (ShowNotificationCenter)
				NotificationCenter.AllRead();
			else
				NotificationCenter.ClearSelection();
		}

		private void ToggleShowSettings()
		{
			Log.Info().Message($"Toggling settings. Value: {!ShowSettings}").Write();
			ShowSettings = !ShowSettings;

			if (ShowSettings && ShowNotificationCenter)
				ShowNotificationCenter = false;

			if (ShowSettings)
				SettingsViewModel.UpdateUser();
		}

		private void TrayIconOnExitRequested(object? sender, EventArgs e)
		{
			Application.Current.Shutdown(0);
		}

		private void TrayIconOnShowWindowRequested(object? sender, EventArgs e)
		{
			Application.Current.Dispatcher?.Invoke(BringWindowToFront);
		}

		private async Task UpdateApp(IAppUpdater? updater = null)
		{
			Log.Info().Message("Checking for updates...").Write();

			try
			{
				var includePreReleases = _coreSetup.Configuration.UsePreReleases;
				updater ??= new AppUpdater(includePreReleases, NotificationCenter, _updateUrls);

				var result = await updater.CheckForUpdates();
				if (result?.ReleasesToApply != null)
				{
					if (!SemVersion.TryParse(result.CurrentVersion, out var currentVersion))
						currentVersion = new SemVersion(0);

					var versions = result.ReleasesToApply.Select(r => SemVersion.TryParse(r.Version, out var version) ? version : new SemVersion(0));
					if (!includePreReleases)
						versions = versions.WhereNotNull().Where(v => string.IsNullOrEmpty(v.Prerelease));

					var newestVersion = versions.OrderByDescending(x => x).FirstOrDefault();
					if (newestVersion != null && newestVersion > currentVersion)
					{
						Log.Info().Message($"Updating to version {result.FutureVersion}").Write();
						await updater.PerformUpdate();
						Log.Info().Message("Update finished").Write();
					}
				}
			}
			catch (Exception ex)
			{
				Log.Warn().Message("Update check failed").Exception(ex).Write();
			}
		}

		private void UpdateNow()
		{
			_cancellationTokenSource?.Cancel();
			_cancellationTokenSource = new CancellationTokenSource();
		}

		private async Task UpdateTimer()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			var stopwatch = new Stopwatch();

			var circuitBreakerTimeout = 10 * UpdateInterval();
			var retryPolicy = Policy.HandleResult(false).Or<Exception>().CircuitBreakerAsync(3, TimeSpan.FromSeconds(circuitBreakerTimeout));

			while (_keepUpdating)
			{
				stopwatch.Restart();
				Log.Debug().Message($"Starting update at UTC: {DateTime.UtcNow}.").Write();

				ResetError();

				if (_coreSetup.Configuration.ShowBusyIndicatorOnDeltaUpdates || _isInitialFetch)
					StatusIndicator.Busy();

				_isInitialFetch = false;

				try
				{
					if (retryPolicy.CircuitState != CircuitState.Open)
						await retryPolicy.ExecuteAsync(async () => await _coreSetup.Update(_previouslyFetchedAnyBuilds ? UpdateModes.DeltaBuilds : UpdateModes.AllBuilds));
					else
						Log.Debug().Message("Previous pipeline updates failed. Postponing updates for now").Write();
				}
				catch (Exception e)
				{
					Log.Error().Message(e.Message).Exception(e).Write();
					throw;
				}

				if (_postPipelineUpdateTask != null)
					await _postPipelineUpdateTask;

				if (StatusIndicator.UpdateStatus == UpdateStatus.Busy)
					StatusIndicator.ClearStatus();

				try
				{
					var updateInterval = UpdateInterval();
					stopwatch.Stop();
					Log.Debug().Message($"Update finished in {stopwatch.Elapsed.TotalSeconds:F1} seconds. Waiting {updateInterval} seconds until next update.").Write();
					stopwatch.Restart();
					await WaitUntilNextFrameIsRenderedAsync();
					stopwatch.Stop();
					Log.Debug().Message($"Took {stopwatch.ElapsedMilliseconds} ms to render new tree.").Write();
					await Task.Delay(TimeSpan.FromSeconds(updateInterval), _cancellationTokenSource.Token);
				}
				catch (Exception)
				{
					// ignored
				}
			}
		}

		private static int UpdateInterval()
		{
#if DEBUG
			const int updateInterval = 5;
#else
			var updateInterval = _coreSetup.Configuration.UpdateInterval;
#endif
			return updateInterval;
		}

		private async Task UpdateTreeTask(PipelineUpdateEventArgs e)
		{
			if (!_previouslyFetchedAnyBuilds && e.Tree.Children.Any())
			{
				NotificationCenter.ClearAllCommand.Execute(null);

				_previouslyFetchedAnyBuilds = true;
			}

			var buildTreeViewModelFactory = new BuildTreeViewModelFactory();

			var buildTreeViewModel = await buildTreeViewModelFactory.ProduceAsync(e.Tree, BuildTree, GroupAndSortDefinitionsSelection.BuildTreeSortingDefinition);
			if (buildTreeViewModel != BuildTree)
				BuildTree = buildTreeViewModel;

			BuildTree.SortingDefinition = GroupAndSortDefinitionsSelection.BuildTreeSortingDefinition;

			ShowNotifications(e.Notifications);
		}

		public void Blur()
		{
			BlurView = true;
		}

		public void UnBlur()
		{
			BlurView = false;
		}

		public void Dispose()
		{
			_trayIcon.Dispose();
			_cancellationTokenSource?.Dispose();
			_postPipelineUpdateTask?.Dispose();
			_fileWatch.Dispose();
		}

		private readonly WindowSettings _windowSettings;
		private readonly IList<BuildNodeViewModel> _highlightedBuilds = new List<BuildNodeViewModel>();
		private readonly CoreSetup _coreSetup;
		private readonly FileWatchDistributedNotificationReceiver _fileWatch;
		private readonly TrayIconHandle _trayIcon;
		private readonly ConfigurationApplication _configurationApplication;
		private readonly IPopupService _popupService;
		private readonly IUpdateUrls _updateUrls;

		private bool _keepUpdating = true;
		private bool _previouslyFetchedAnyBuilds;
		private bool _blurView;
		private CancellationTokenSource? _cancellationTokenSource;
		private Task? _postPipelineUpdateTask;
		private BuildTreeViewModel? _buildTree;
		private bool _showGroupDefinitionSelection;
		private bool _showSettings;
		private BaseViewModel? _overlay;
		private bool _showNotificationCenter;
		private bool _hasAnyProjects;
		private bool _isInitialFetch = true;
	}
}