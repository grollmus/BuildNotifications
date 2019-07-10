using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BuildNotifications.Core;
using BuildNotifications.Core.Pipeline;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.ViewModel.GroupDefinitionSelection;
using BuildNotifications.ViewModel.Tree;
using BuildNotifications.ViewModel.Tree.Dummy;
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
            _buildTreeSource = new BuildTreeDummy(null);
            //UpdateOrCreateBuildTree();

            _coreSetup = new CoreSetup();

            var projectProvider = _coreSetup.ProjectProvider;
            foreach (var project in projectProvider.AllProjects())
            {
                _coreSetup.Pipeline.AddProject(project);
            }

            _coreSetup.PipelineUpdated += CoreSetup_PipelineUpdated;

            UpdateTimer();
        }

        private async Task UpdateTimer()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(30));
                await _coreSetup.Update();
            }
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
            BuildTree = new BuildTreeViewModelFactory().Produce(e.Tree);
        }

        // RemoveChildrenIfNotOfType ensures that only elements of the type are within the list, therefore the ReSharper warning is taken care of
        [SuppressMessage("ReSharper", "PossibleInvalidCastExceptionInForeachLoop")]
        private void CreateDummys(BuildTreeNodeDummy addToNode, int currentGroupingIndex, List<GroupDefinition> groupDefinitions)
        {
            void RemoveChildrenIfNotOfType<T>(BuildTreeNodeDummy ofNode, int expectedCount) where T : BuildTreeNodeDummy
            {
                if (!ofNode.Children.All(x => x is T))
                    ofNode.Clear();
                else
                {
                    var toRemove = ofNode.Children.Skip(expectedCount).ToList();
                    foreach (var child in toRemove)
                    {
                        ofNode.RemoveChild(child);
                    }
                }
            }

            var childCount = _random.Next(1, 5);

            if (currentGroupingIndex >= groupDefinitions.Count)
            {
                RemoveChildrenIfNotOfType<BuildNodeDummy>(addToNode, childCount);

                for (var i = addToNode.Children.Count(); i < childCount; i++)
                {
                    addToNode.AddChild(new BuildNodeDummy());
                }

                return;
            }

            var groupDef = groupDefinitions[currentGroupingIndex];
            switch (groupDef)
            {
                case GroupDefinition.Branch:
                    RemoveChildrenIfNotOfType<BranchGroupNodeDummy>(addToNode, childCount);

                    for (var i = addToNode.Children.Count(); i < childCount; i++)
                    {
                        var branchGroupNodeDummy = new BranchGroupNodeDummy("Branch " + (char) ('A' + i));
                        addToNode.AddChild(branchGroupNodeDummy);
                    }

                    foreach (BranchGroupNodeDummy child in addToNode.Children)
                    {
                        CreateDummys(child, currentGroupingIndex + 1, groupDefinitions);
                    }

                    break;
                case GroupDefinition.BuildDefinition:
                    RemoveChildrenIfNotOfType<DefinitionGroupNodeDummy>(addToNode, childCount);

                    for (var i = addToNode.Children.Count(); i < childCount; i++)
                    {
                        var definitionGroupNodeDummy = new DefinitionGroupNodeDummy("Definition " + (i + 1));
                        CreateDummys(definitionGroupNodeDummy, currentGroupingIndex + 1, groupDefinitions);
                        addToNode.AddChild(definitionGroupNodeDummy);
                    }

                    foreach (DefinitionGroupNodeDummy child in addToNode.Children)
                    {
                        CreateDummys(child, currentGroupingIndex + 1, groupDefinitions);
                    }

                    break;
                case GroupDefinition.Source:
                    RemoveChildrenIfNotOfType<SourceGroupNodeDummy>(addToNode, childCount);

                    for (var i = addToNode.Children.Count(); i < childCount; i++)
                    {
                        var sourceGroupNodeDummy = new SourceGroupNodeDummy("Source " + i);
                        CreateDummys(sourceGroupNodeDummy, currentGroupingIndex + 1, groupDefinitions);
                        addToNode.AddChild(sourceGroupNodeDummy);
                    }

                    foreach (SourceGroupNodeDummy child in addToNode.Children)
                    {
                        CreateDummys(child, currentGroupingIndex + 1, groupDefinitions);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RandomTree()
        {
            var groupings = GroupAndSortDefinitionsSelection.BuildTreeGroupDefinition.ToList();
            CreateDummys(_buildTreeSource, 0, groupings);
            var buildTreeGroupDefinition = new GroupDefinitionDummy();
            buildTreeGroupDefinition.AddRange(groupings);
            _buildTreeSource.GroupDefinition = buildTreeGroupDefinition;
        }

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

        private readonly BuildTreeDummy _buildTreeSource;
        private BuildTreeViewModel _buildTree;
        private bool _showGroupDefinitionSelection;

        private static readonly Random _random = new Random();
        private readonly CoreSetup _coreSetup;
    }
}