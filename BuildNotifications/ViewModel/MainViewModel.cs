using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Anotar.NLog;
using BuildNotifications.Core;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Notification;
using BuildNotifications.Services;
using BuildNotifications.Core.Pipeline.Notification.Distribution;
using BuildNotifications.Core.Protocol;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.GroupDefinitionSelection;
using BuildNotifications.ViewModel.Notification;
using BuildNotifications.ViewModel.Overlays;
using BuildNotifications.ViewModel.Settings;
using BuildNotifications.ViewModel.Tree;
using BuildNotifications.ViewModel.Utils;
using JetBrains.Annotations;
using Semver;
using ToastNotificationsPlugin;
using TweenSharp.Animation;
using TweenSharp.Factory;

// properties *are* initialized within the constructor. However by a method call, which is not correctly recognized by the code analyzer yet.
#pragma warning disable CS8618 // warning about uninitialized non-nullable properties
namespace BuildNotifications.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            var pathResolver = new PathResolver();
            _fileWatch = new FileWatchDistributedNotificationReceiver(pathResolver);
            _trayIcon = new TrayIconHandle();
            _trayIcon.ExitRequested += TrayIconOnExitRequested;
            _trayIcon.ShowWindowRequested += TrayIconOnShowWindowRequested;
            _coreSetup = new CoreSetup(pathResolver, _fileWatch);
            _coreSetup.PipelineUpdated += CoreSetup_PipelineUpdated;
            _coreSetup.DistributedNotificationReceived += CoreSetup_DistributedNotificationReceived;
            GlobalErrorLogTarget.ErrorOccured += GlobalErrorLog_ErrorOccurred;
            Initialize();
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

        private async void CoreSetup_PipelineUpdated(object? sender, PipelineUpdateEventArgs e)
        {
            var buildTreeViewModelFactory = new BuildTreeViewModelFactory();

            var buildTreeViewModel = await buildTreeViewModelFactory.ProduceAsync(e.Tree, BuildTree);
            if (buildTreeViewModel != BuildTree)
                BuildTree = buildTreeViewModel;

            BuildTree.SortingDefinition = GroupAndSortDefinitionsSelection.BuildTreeSortingDefinition;

            NotificationCenter.ShowNotifications(e.Notifications);
        }

        private void GlobalErrorLog_ErrorOccurred(object? sender, ErrorNotificationEventArgs e)
        {
            StopUpdating();
            StatusIndicator.Error(e.ErrorNotifications);

            // errors may occur on any thread. 
            Application.Current.Dispatcher?.Invoke(() => { NotificationCenter.ShowNotifications(e.ErrorNotifications); });
        }

        private void GroupAndSortDefinitionsSelectionOnPropertyChanged(object sender, PropertyChangedEventArgs args)
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

        private void Initialize()
        {
            SetupViewModel();
            LoadProjects();
            ShowOverlay();
            RegisterUriProtocol();
            HandleExistingDistributedNotificationsOnNextFrame();
            UpdateApp().FireAndForget();

            if (Overlay == null)
                StartUpdating();
        }

        private class Dummy
        {
            [UsedImplicitly]
            public int DummyProp { get; set; }
        }

        private void HandleExistingDistributedNotificationsOnNextFrame()
        {
            // start a tween which will finish as soon as the next frame is rendered (because this is when tweens get updated.)

            App.GlobalTweenHandler.Add(new Dummy().Tween(x => x.DummyProp).To(0).In(0).OnComplete((sender, parameter) =>
            {
                // remove any image files that may still exist from last launch
                NotificationDistributor.DeleteAllTemporaryImageFiles();
                // set initial tray icon (do this here, so any instance which immediately exits doesn't spawn an icon)
                _trayIcon.BuildStatus = BuildStatus.None;
                _fileWatch.HandleAllExistingFiles();
            }));
        }

        private void RegisterUriProtocol() => UriSchemeRegistration.Register();

        private void InitialSetup_CloseRequested(object? sender, InitialSetupEventArgs e)
        {
            if (!(sender is InitialSetupOverlayViewModel vm))
                return;

            vm.CloseRequested -= InitialSetup_CloseRequested;
            var tween = vm.Tween(x => x.Opacity).To(0).In(0.5).Ease(Easing.ExpoEaseOut).OnComplete((timeline, parameter) =>
            {
                Overlay = null;

                // when connections or projects changed or the update is stopped. Now is the time to reload and restart the pipeline
                // as the user either changed or checked the critical settings
                if (e.ProjectOrConnectionsChanged || !_keepUpdating)
                    ResetAndRestart();

                StartUpdating();
            });

            App.GlobalTweenHandler.Add(tween);
        }

        private void LoadProjects()
        {
            _coreSetup.Pipeline.ClearProjects();
            var projectProvider = _coreSetup.ProjectProvider;
            foreach (var project in projectProvider.AllProjects())
            {
                _coreSetup.Pipeline.AddProject(project);
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

            NotificationCenter.ClearNotificationsOfType(NotificationType.Error);
        }

        private void SettingsViewModelOnEditConnectionsRequested(object? sender, EventArgs e)
        {
            ToggleShowSettingsCommand.Execute(null);
            ShowInitialSetupOverlayViewModel();
        }

        private void SetupViewModel()
        {
            SearchViewModel = new SearchViewModel(_coreSetup.Pipeline);
            StatusIndicator = new StatusIndicatorViewModel();
            StatusIndicator.ResumeRequested += StatusIndicator_OnResumeRequested;
            StatusIndicator.OpenErrorMessageRequested += StatusIndicator_OnOpenErrorMessageRequested;

            SetupNotificationCenter();

            SettingsViewModel = new SettingsViewModel(_coreSetup.Configuration, () => _coreSetup.PersistConfigurationChanges(), _coreSetup.PluginRepository);
            SettingsViewModel.EditConnectionsRequested += SettingsViewModelOnEditConnectionsRequested;

            GroupAndSortDefinitionsSelection = new GroupAndSortDefinitionsViewModel
            {
                BuildTreeGroupDefinition = _coreSetup.Configuration.GroupDefinition,
                BuildTreeSortingDefinition = _coreSetup.Configuration.SortingDefinition
            };
            GroupAndSortDefinitionsSelection.PropertyChanged += GroupAndSortDefinitionsSelectionOnPropertyChanged;

            ToggleGroupDefinitionSelectionCommand = new DelegateCommand(ToggleGroupDefinitionSelection);
            ToggleShowSettingsCommand = new DelegateCommand(ToggleShowSettings);
            ToggleShowNotificationCenterCommand = new DelegateCommand(ToggleShowNotificationCenter);
        }

        private void SetupNotificationCenter()
        {
            NotificationCenter = new NotificationCenterViewModel();

            var toastNotificationProcessor = new ToastNotificationProcessor();

            NotificationCenter.NotificationDistributor.Add(toastNotificationProcessor);
            NotificationCenter.NotificationDistributor.Add(_trayIcon);
            NotificationCenter.HighlightRequested += NotificationCenterOnHighlightRequested;
        }

        private void TrayIconOnShowWindowRequested(object? sender, EventArgs e)
        {
            Application.Current.Dispatcher?.Invoke(BringWindowToFront);
        }

        private void TrayIconOnExitRequested(object? sender, EventArgs e)
        {
            Application.Current.Shutdown(0);
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
                        NotificationCenter.ShowNotifications(new List<INotification> {new StatusNotification(e.DistributedNotification.BasedOnNotification.Value.ToString(), StringLocalizer.Instance["NotificationNotFound"], NotificationType.Info)});

                    if (!ShowNotificationCenter)
                        ToggleShowNotificationCenter(this);
                }
            });
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
            }
        }

        private void ShowInitialSetupOverlayViewModel()
        {
            if (Overlay != null)
                return;

            StopUpdating();
            var vm = new InitialSetupOverlayViewModel(SettingsViewModel, _coreSetup.PluginRepository);
            vm.CloseRequested += InitialSetup_CloseRequested;

            Overlay = vm;
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
            if (_keepUpdating)
                return;
            _keepUpdating = true;
            UpdateTimer().FireAndForget();
            _fileWatch.Start();

            StatusIndicator.Resume();
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
            _keepUpdating = false;
            UpdateNow(); // cancels the wait timer
            _fileWatch.Stop();
            StatusIndicator.Pause();
        }

        private void ToggleGroupDefinitionSelection(object obj)
        {
            ShowGroupDefinitionSelection = !ShowGroupDefinitionSelection;
        }

        private void ToggleShowNotificationCenter(object obj)
        {
            ShowNotificationCenter = !ShowNotificationCenter;
            if (ShowSettings && ShowNotificationCenter)
                ShowSettings = false;

            if (ShowNotificationCenter)
            {
                StatusIndicator.ClearStatus();
            
                // reset error icon when user opens window, as the point of the error icon is to notify the user.
                _trayIcon.BuildStatus = BuildStatus.None;
            }
            else
                NotificationCenter.ClearSelection();
        }

        private void ToggleShowSettings(object obj)
        {
            ShowSettings = !ShowSettings;
            if (ShowSettings && ShowNotificationCenter)
                ShowNotificationCenter = false;
        }

        private async Task UpdateApp()
        {
            LogTo.Info("Checking for updates...");

            try
            {
                var includePreReleases = _coreSetup.Configuration.UsePreReleases;
                var updater = new AppUpdater();

                var result = await updater.CheckForUpdates();
                if (result != null)
                {
                    if (!SemVersion.TryParse(result.CurrentVersion, out var currentVersion))
                        currentVersion = new SemVersion(0);

                    var versions = result.ReleasesToApply.Select(r => SemVersion.TryParse(r.Version, out var version) ? version : new SemVersion(0));
                    if (!includePreReleases)
                        versions = versions.Where(v => string.IsNullOrEmpty(v.Prerelease));

                    var newestVersion = versions.OrderByDescending(x => x).FirstOrDefault();
                    if (newestVersion != null && newestVersion > currentVersion)
                    {
                        LogTo.Info($"Updating to version {result.FutureVersion}");
                        await updater.PerformUpdate();
                        LogTo.Info("Update finished");
                    }
                }
            }
            catch (Exception ex)
            {
                LogTo.WarnException("Update check failed", ex);
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
            while (_keepUpdating)
            {
                if (StatusIndicator.UpdateStatus == UpdateStatus.None)
                    StatusIndicator.Busy();

                await _coreSetup.Update();

                if (StatusIndicator.UpdateStatus == UpdateStatus.Busy)
                    StatusIndicator.ClearStatus();

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), _cancellationTokenSource.Token);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        private readonly IList<BuildNodeViewModel> _highlightedBuilds = new List<BuildNodeViewModel>();
        private readonly CoreSetup _coreSetup;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _keepUpdating;
        private BuildTreeViewModel? _buildTree;
        private bool _showGroupDefinitionSelection;
        private bool _showSettings;
        private BaseViewModel? _overlay;
        private bool _showNotificationCenter;
        private readonly FileWatchDistributedNotificationReceiver _fileWatch;
        private TrayIconHandle _trayIcon;
    }
}
#pragma warning enable CS8618