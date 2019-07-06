using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree.Arrangement;
using BuildNotifications.ViewModel.GroupDefinitionSelection;
using BuildNotifications.ViewModel.Tree;
using BuildNotifications.ViewModel.Tree.Dummy;
using BuildNotifications.ViewModel.Utils;

namespace BuildNotifications.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private BuildTreeViewModel _buildTree;

        public BuildTreeViewModel BuildTree {
            get => _buildTree;
            set {
                _buildTree = value;
                OnPropertyChanged();
            }
        }

        public SearchViewModel SearchViewModel { get; set; }

        public ICommand LoadNewRandomTreeCommand { get; set; }

        public ICommand ToggleGroupDefinitionSelectionCommand { get; set; }

        public GroupAndSortDefinitionsViewModel GroupAndSortDefinitionsSelection { get; set; }

        public bool ShowGroupDefinitionSelection
        {
            get => _showGroupDefinitionSelection;
            set
            {
                _showGroupDefinitionSelection = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            SearchViewModel = new SearchViewModel();
            GroupAndSortDefinitionsSelection = new GroupAndSortDefinitionsViewModel();
            LoadNewRandomTreeCommand = new DelegateCommand(LoadNewRandomTree);
            ToggleGroupDefinitionSelectionCommand = new DelegateCommand(ToggleGroupDefinitionSelection);
            _buildTreeSource = new BuildTreeDummy(null);
            LoadNewRandomTree(null);
        }

        private void ToggleGroupDefinitionSelection(object obj)
        {
            ShowGroupDefinitionSelection = !ShowGroupDefinitionSelection;
        }

        private readonly BuildTreeDummy _buildTreeSource;
        private bool _showGroupDefinitionSelection;

        private async void LoadNewRandomTree(object obj)
        {
            IsBusy = true;
            await Task.Delay(2500);
            BuildTree = await Task.Run(() =>
            {
                CreateRandomTree();
                var factory = new BuildTreeViewModelFactory();
                return factory.Produce(_buildTreeSource);
            });
            IsBusy = false;
        }

        private void CreateRandomTree()
        {
            var groupings = RandomGroupings().ToList();
            Debug.WriteLine(string.Join(',', groupings));
            CreateDummys(_buildTreeSource, 0, groupings);
            var buildTreeGroupDefinition = new GroupDefinitionDummy();
            buildTreeGroupDefinition.AddRange(groupings);
            _buildTreeSource.GroupDefinition = buildTreeGroupDefinition;
        }

        private IEnumerable<GroupDefinition> RandomGroupings()
        {
            return GroupAndSortDefinitionsSelection.BuildTreeGroupDefinition
        }

        private void CreateDummys(BuildTreeNodeDummy addToNode, int currentGroupingIndex, List<GroupDefinition> groupDefinitions)
        {
            void RemoveChildrenIfNotOfType<T>() where T : BuildTreeNodeDummy
            {
                if (!addToNode.Children.Any(x => x is T))
                    addToNode.Clear();
            }

            if (currentGroupingIndex >= groupDefinitions.Count)
            {
                RemoveChildrenIfNotOfType<BuildNodeDummy>();

                for (var i = addToNode.Children.Count(); i < 3; i++)
                {
                    addToNode.AddChild(new BuildNodeDummy());
                }

                return;
            }

            var groupDef = groupDefinitions[currentGroupingIndex];
            switch (groupDef)
            {
                case GroupDefinition.Branch:
                    RemoveChildrenIfNotOfType<BranchGroupNodeDummy>();

                    for (var i = addToNode.Children.Count(); i < 4; i++)
                    {
                        var branchGroupNodeDummy = new BranchGroupNodeDummy("Branch " + (char)('A' + i));
                        CreateDummys(branchGroupNodeDummy, currentGroupingIndex + 1, groupDefinitions);
                        addToNode.AddChild(branchGroupNodeDummy);
                    }

                    break;
                case GroupDefinition.BuildDefinition:
                    RemoveChildrenIfNotOfType<DefinitionGroupNodeDummy>();

                    for (var i = addToNode.Children.Count(); i < 4; i++)
                    {
                        var definitionGroupNodeDummy = new DefinitionGroupNodeDummy("Definition " + (i + 1));
                        CreateDummys(definitionGroupNodeDummy, currentGroupingIndex + 1, groupDefinitions);
                        addToNode.AddChild(definitionGroupNodeDummy);
                    }

                    break;
                case GroupDefinition.Source:
                    RemoveChildrenIfNotOfType<SourceGroupNodeDummy>();

                    for (var i = addToNode.Children.Count(); i < 2; i++)
                    {
                        var sourceGroupNodeDummy = new SourceGroupNodeDummy("Source " + i);
                        CreateDummys(sourceGroupNodeDummy, currentGroupingIndex + 1, groupDefinitions);
                        addToNode.AddChild(sourceGroupNodeDummy);
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}