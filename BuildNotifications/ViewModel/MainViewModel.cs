using System.Collections.Generic;
using System.Threading.Tasks;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.ViewModel.Tree;
using BuildNotifications.ViewModel.Tree.Dummy;

namespace BuildNotifications.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private BuildTreeViewModel _buildTree;

        public BuildTreeViewModel BuildTree
        {
            get => _buildTree;
            set
            {
                _buildTree = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            Initialize();
        }

        private async void Initialize()
        {
            IsBusy = true;
            await Task.Delay(6000);
            BuildTree = await Task.Run(() =>
            {
                var factory = new BuildTreeViewModelFactory();
                return factory.Produce(DummyBuildTree());
            });
            IsBusy = false;
        }

        private IBuildTree DummyBuildTree()
        {
            var source1Branch1DefABuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
            var source1Branch1DefBBuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
            var source1Branch2DefABuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
            var source1Branch3DefABuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
            var source1Branch4DefABuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
            var source1Branch3DefBBuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
            var source1Branch4DefBBuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
            var source2Branch1DefABuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};
            var source2Branch1DefBBuilds = new List<IBuildNode> {new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy()};

            var sources = new List<SourceGroupNodeDummy> {new SourceGroupNodeDummy(), new SourceGroupNodeDummy()};

            var source1Branch1 = new BranchGroupNodeDummy("Source 1 Branch 1");
            var source1Branch2 = new BranchGroupNodeDummy("Source 1 Branch 2");
            var source1Branch3 = new BranchGroupNodeDummy("Source 1 Branch 4");
            var source1Branch4 = new BranchGroupNodeDummy("Source 1 Branch 5");
            var source2Branch1 = new BranchGroupNodeDummy("Source 2 Branch 1");

            var source1Branch1DefA = new DefinitionGroupNodeDummy("Definition A");
            var source1Branch1DefB = new DefinitionGroupNodeDummy("Definition B");
            var source1Branch3DefA = new DefinitionGroupNodeDummy("Definition A");
            var source1Branch3DefB = new DefinitionGroupNodeDummy("Definition B");
            var source1Branch4DefA = new DefinitionGroupNodeDummy("Definition A");
            var source1Branch4DefB = new DefinitionGroupNodeDummy("Definition B");
            var source1Branch2DefA = new DefinitionGroupNodeDummy("Definition A");
            var source2Branch1DefA = new DefinitionGroupNodeDummy("Definition A");
            var source2Branch1DefB = new DefinitionGroupNodeDummy("Definition B");

            source1Branch1DefA.AddRange(source1Branch1DefABuilds);
            source1Branch1DefB.AddRange(source1Branch1DefBBuilds);
            source1Branch3DefA.AddRange(source1Branch3DefABuilds);
            source1Branch3DefB.AddRange(source1Branch3DefBBuilds);
            source1Branch4DefA.AddRange(source1Branch4DefABuilds);
            source1Branch4DefB.AddRange(source1Branch4DefBBuilds);
            source1Branch2DefA.AddRange(source1Branch2DefABuilds);
            source2Branch1DefA.AddRange(source2Branch1DefABuilds);
            source2Branch1DefB.AddRange(source2Branch1DefBBuilds);

            source1Branch1.AddRange(new[] {source1Branch1DefA, source1Branch1DefB});
            source1Branch3.AddRange(new[] {source1Branch3DefA, source1Branch3DefB});
            source1Branch4.AddRange(new[] {source1Branch4DefA, source1Branch4DefB});
            source1Branch2.AddRange(new[] {source1Branch2DefA});
            source2Branch1.AddRange(new[] {source2Branch1DefA, source2Branch1DefB});

            sources[0].AddChild(source1Branch1);
            sources[0].AddChild(source1Branch2);
            sources[0].AddChild(source1Branch3);
            sources[0].AddChild(source1Branch4);
            sources[1].AddChild(source2Branch1);

            var tree = new BuildTreeDummy(new GroupDefinitionDummy {GroupDefinition.Source, GroupDefinition.Branch, GroupDefinition.BuildDefinition});
            tree.AddRange(sources);

            return tree;
        }
    }
}