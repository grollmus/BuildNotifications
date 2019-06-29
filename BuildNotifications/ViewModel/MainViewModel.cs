using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BuildNotifications.Core.Pipeline.Tree;
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

        public BuildTreeGroupDefinitionViewModel GroupDefinitionSelection { get; set; }

        public MainViewModel()
        {
            SearchViewModel = new SearchViewModel();
            GroupDefinitionSelection = new BuildTreeGroupDefinitionViewModel();
            LoadNewRandomTreeCommand = new DelegateCommand(LoadNewRandomTree);
            _buildTreeSource = new BuildTreeDummy(null);
            LoadNewRandomTree(null);
        }

        private readonly BuildTreeDummy _buildTreeSource;

        private async void LoadNewRandomTree(object obj)
        {
            IsBusy = true;
            await Task.Delay(600);
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
            return new List<GroupDefinition> {GroupDefinition.Branch, GroupDefinition.Source, GroupDefinition.BuildDefinition};
            var allGroupDefinitions = new List<GroupDefinition> { GroupDefinition.Branch, GroupDefinition.BuildDefinition, GroupDefinition.Source };
            var rnd = new Random();
            var length = rnd.Next(allGroupDefinitions.Count + 1);
            length = 3;
            var randomGroupDefinitions = new List<GroupDefinition>(length);
            for (var i = 0; i < length; i++)
            {
                var rndGroup = allGroupDefinitions[rnd.Next(allGroupDefinitions.Count)];
                allGroupDefinitions.Remove(rndGroup);
                randomGroupDefinitions.Add(rndGroup);
            }

            return randomGroupDefinitions;
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

        //private IBuildTree DummyBuildTree()
        //{
        //    var source1Branch1DefABuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
        //    var source1Branch1DefBBuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
        //    var source1Branch2DefABuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
        //    var source1Branch3DefABuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
        //    var source1Branch4DefABuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
        //    var source1Branch3DefBBuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
        //    var source1Branch4DefBBuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
        //    var source2Branch1DefABuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
        //    var source2Branch1DefBBuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};

        //    var sources = new List<SourceGroupNodeDummy> {new SourceGroupNodeDummy("S"), new SourceGroupNodeDummy()};

        //    var source1Branch1 = new BranchGroupNodeDummy("Source 1 Branch 1");
        //    var source1Branch2 = new BranchGroupNodeDummy("Source 1 Branch 2");
        //    var source1Branch3 = new BranchGroupNodeDummy("Source 1 Branch 4");
        //    var source1Branch4 = new BranchGroupNodeDummy("Source 1 Branch 5");
        //    var source2Branch1 = new BranchGroupNodeDummy("Source 2 Branch 1");

        //    var source1Branch1DefA = new DefinitionGroupNodeDummy("Definition A");
        //    var source1Branch1DefB = new DefinitionGroupNodeDummy("Definition B");
        //    var source1Branch3DefA = new DefinitionGroupNodeDummy("Definition A");
        //    var source1Branch3DefB = new DefinitionGroupNodeDummy("Definition B");
        //    var source1Branch4DefA = new DefinitionGroupNodeDummy("Definition A");
        //    var source1Branch4DefB = new DefinitionGroupNodeDummy("Definition B");
        //    var source1Branch2DefA = new DefinitionGroupNodeDummy("Definition A");
        //    var source2Branch1DefA = new DefinitionGroupNodeDummy("Definition A");
        //    var source2Branch1DefB = new DefinitionGroupNodeDummy("Definition B");

        //    source1Branch1DefA.AddRange(source1Branch1DefABuilds);
        //    source1Branch1DefB.AddRange(source1Branch1DefBBuilds);
        //    source1Branch3DefA.AddRange(source1Branch3DefABuilds);
        //    source1Branch3DefB.AddRange(source1Branch3DefBBuilds);
        //    source1Branch4DefA.AddRange(source1Branch4DefABuilds);
        //    source1Branch4DefB.AddRange(source1Branch4DefBBuilds);
        //    source1Branch2DefA.AddRange(source1Branch2DefABuilds);
        //    source2Branch1DefA.AddRange(source2Branch1DefABuilds);
        //    source2Branch1DefB.AddRange(source2Branch1DefBBuilds);

        //    source1Branch1.AddRange(new[] {source1Branch1DefA, source1Branch1DefB});
        //    source1Branch3.AddRange(new[] {source1Branch3DefA, source1Branch3DefB});
        //    source1Branch4.AddRange(new[] {source1Branch4DefA, source1Branch4DefB});
        //    source1Branch2.AddRange(new[] {source1Branch2DefA});
        //    source2Branch1.AddRange(new[] {source2Branch1DefA, source2Branch1DefB});

        //    sources[0].AddChild(source1Branch1);
        //    sources[0].AddChild(source1Branch2);
        //    sources[0].AddChild(source1Branch3);
        //    sources[0].AddChild(source1Branch4);
        //    sources[1].AddChild(source2Branch1);

        //    var tree = new BuildTreeDummy(new GroupDefinitionDummy {GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition});
        //    tree.AddRange(sources);

        //    return tree;
        //}
    }
}