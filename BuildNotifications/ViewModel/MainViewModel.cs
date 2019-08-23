using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using BuildNotifications.Core;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.ViewModel.GroupDefinitionSelection;
using BuildNotifications.ViewModel.Overlays;
using BuildNotifications.ViewModel.Settings;
using BuildNotifications.ViewModel.Tree;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private const string ConfigFileName = "config.json";
#if DEBUG
        private string ConfigFilePath => ConfigFileName;
#else
        private string ConfigFilePath => System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), $"BuildNotifications{System.IO.Path.DirectorySeparatorChar}{ConfigFileName}");
#endif

        public MainViewModel()
        {
            _coreSetup = new CoreSetup(ConfigFilePath);
            Initialize();
        }

        public BuildTreeViewModel BuildTree
        {
            get => _buildTree;
            set
            {
                _buildTree = value;
                OnPropertyChanged();
            }
        }

        public BaseViewModel Overlay
        {
            get => _overlay;
            set
            {
                _overlay = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TitleBarToolsVisibility));
            }
        }

        public Visibility TitleBarToolsVisibility => Overlay == null ? Visibility.Visible : Visibility.Collapsed;

        public GroupAndSortDefinitionsViewModel GroupAndSortDefinitionsSelection { get; set; }

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

        public bool ShowSettings
        {
            get => _showSettings;
            set
            {
                _showSettings = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleGroupDefinitionSelectionCommand { get; set; }
        public ICommand ToggleShowSettingsCommand { get; set; }

        private void Initialize()
        {
            SetupViewModel();
            LoadProjects();
            _coreSetup.PipelineUpdated += CoreSetup_PipelineUpdated;
            UpdateTimer().FireAndForget();
            ShowOverlay();
        }

        private void ShowOverlay()
        {
            var nothingConfigured = !_coreSetup.Configuration.Projects.Any() || !_coreSetup.Configuration.Connections.Any();
            if (!nothingConfigured)
                return;

            Overlay = new InitialSetupOverlayViewModel(SettingsViewModel, _coreSetup.PluginRepository);
        }

        private void LoadProjects()
        {
            var projectProvider = _coreSetup.ProjectProvider;
            foreach (var project in projectProvider.AllProjects())
            {
                _coreSetup.Pipeline.AddProject(project);
            }
        }

        private void SetupViewModel()
        {
            SearchViewModel = new SearchViewModel();
            SettingsViewModel = new SettingsViewModel(_coreSetup.Configuration, () => _coreSetup.PersistConfigurationChanges());
            SettingsViewModel.EditConnectionsRequested += SettingsViewModelOnEditConnectionsRequested;

            GroupAndSortDefinitionsSelection = new GroupAndSortDefinitionsViewModel();
            GroupAndSortDefinitionsSelection.BuildTreeGroupDefinition = _coreSetup.Configuration.GroupDefinition;
            GroupAndSortDefinitionsSelection.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(GroupAndSortDefinitionsViewModel.BuildTreeGroupDefinition))
                {
                    _coreSetup.Configuration.GroupDefinition = GroupAndSortDefinitionsSelection.BuildTreeGroupDefinition;
                    _coreSetup.PersistConfigurationChanges();
                    UpdateNow();
                }

                if (args.PropertyName == nameof(GroupAndSortDefinitionsViewModel.BuildTreeSortingDefinition))
                    BuildTree.SortingDefinition = GroupAndSortDefinitionsSelection.BuildTreeSortingDefinition;
            };

            ToggleGroupDefinitionSelectionCommand = new DelegateCommand(ToggleGroupDefinitionSelection);
            ToggleShowSettingsCommand = new DelegateCommand(ToggleShowSettings);
        }

        private void SettingsViewModelOnEditConnectionsRequested(object sender, EventArgs e)
        {
            ToggleShowSettingsCommand.Execute(null);
            Overlay = new InitialSetupOverlayViewModel(SettingsViewModel, _coreSetup.PluginRepository);
        }

        private async void CoreSetup_PipelineUpdated(object sender, PipelineUpdateEventArgs e)
        {
            var buildTreeViewModelFactory = new BuildTreeViewModelFactory();
            
            var buildTreeViewModel = await buildTreeViewModelFactory.ProduceAsync(e.Tree, BuildTree);
            if (buildTreeViewModel != BuildTree)
                BuildTree = buildTreeViewModel;

            BuildTree.SortingDefinition = GroupAndSortDefinitionsSelection.BuildTreeSortingDefinition;
        }

        private void ToggleGroupDefinitionSelection(object obj)
        {
            ShowGroupDefinitionSelection = !ShowGroupDefinitionSelection;
        }

        private void ToggleShowSettings(object obj)
        {
            ShowSettings = !ShowSettings;
        }

        private async Task UpdateTimer()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            while (_keepUpdating)
            {
                IsBusy = true;
                await _coreSetup.Update();
                IsBusy = false;
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

        private void UpdateNow()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        private CancellationTokenSource _cancellationTokenSource;
        private bool _keepUpdating = true;
        private readonly CoreSetup _coreSetup;
        private BuildTreeViewModel _buildTree;
        private bool _showGroupDefinitionSelection;
        private bool _showSettings;
        private BaseViewModel _overlay;
    }
}