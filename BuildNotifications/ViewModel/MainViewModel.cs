using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using BuildNotifications.Core;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.ViewModel.GroupDefinitionSelection;
using BuildNotifications.ViewModel.Tree;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public MainViewModel()
        {
            SearchViewModel = new SearchViewModel();

            GroupAndSortDefinitionsSelection = new GroupAndSortDefinitionsViewModel();
            GroupAndSortDefinitionsSelection.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(GroupAndSortDefinitionsViewModel.BuildTreeGroupDefinition))
                {
                    Debug.WriteLine("Selected groups: " + string.Join(',', GroupAndSortDefinitionsSelection.BuildTreeGroupDefinition));
                    UpdateOrCreateBuildTree();
                }

                if (args.PropertyName == nameof(GroupAndSortDefinitionsViewModel.BuildTreeSortingDefinition))
                {
                    Debug.WriteLine("Selected sortings: " + string.Join(',', GroupAndSortDefinitionsSelection.BuildTreeSortingDefinition));
                    BuildTree.SortingDefinition = GroupAndSortDefinitionsSelection.BuildTreeSortingDefinition;
                }
            };

            LoadNewRandomTreeCommand = new DelegateCommand(UpdateOrCreateBuildTree);
            ToggleGroupDefinitionSelectionCommand = new DelegateCommand(ToggleGroupDefinitionSelection);

            _coreSetup = new CoreSetup();

            var projectProvider = _coreSetup.ProjectProvider;
            foreach (var project in projectProvider.AllProjects())
            {
                _coreSetup.Pipeline.AddProject(project);
            }

            _coreSetup.PipelineUpdated += CoreSetup_PipelineUpdated;

            UpdateTimer();
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

        public GroupAndSortDefinitionsViewModel GroupAndSortDefinitionsSelection { get; set; }

        public ICommand LoadNewRandomTreeCommand { get; set; }

        public SearchViewModel SearchViewModel { get; set; }

        public bool ShowGroupDefinitionSelection
        {
            get => _showGroupDefinitionSelection;
            set
            {
                _showGroupDefinitionSelection = value;
                OnPropertyChanged();
            }
        }

        public ICommand ToggleGroupDefinitionSelectionCommand { get; set; }

        private void CoreSetup_PipelineUpdated(object sender, PipelineUpdateEventArgs e)
        {
            var buildTreeViewModelFactory = new BuildTreeViewModelFactory();

            BuildTree = buildTreeViewModelFactory.Produce(e.Tree, BuildTree);
        }

        // RemoveChildrenIfNotOfType ensures that only elements of the type are within the list, therefore the ReSharper warning is taken care of

        private void ToggleGroupDefinitionSelection(object obj)
        {
            ShowGroupDefinitionSelection = !ShowGroupDefinitionSelection;
        }

        private async void UpdateOrCreateBuildTree(object obj = null)
        {
            //IsBusy = true;
            //BuildTree = await Task.Run(() =>
            //{
            //    RandomTree();
            //    var factory = new BuildTreeViewModelFactory();
            //    return factory.Produce(_buildTreeSource);
            //});
            //IsBusy = false;
        }

        private async Task UpdateTimer()
        {
            while (true)
            {
                IsBusy = true;
                await _coreSetup.Update();
                IsBusy = false;

                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }

        private readonly CoreSetup _coreSetup;
        private BuildTreeViewModel _buildTree;
        private bool _showGroupDefinitionSelection;
    }
}