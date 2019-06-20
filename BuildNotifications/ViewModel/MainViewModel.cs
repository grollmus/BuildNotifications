using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.ViewModel.Tree;
using BuildNotifications.ViewModel.Tree.Dummy;

namespace BuildNotifications.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        public BuildTreeViewModel BuildTree { get; set; }

        public MainViewModel()
        {
            var factory = new BuildTreeViewModelFactory();
            BuildTree = factory.Produce(DummyBuildTree());
        }

        private IBuildTree DummyBuildTree()
        {
            var source1Branch1Builds = new List<IBuildNode> { new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy() };
            var source1Branch2Builds = new List<IBuildNode> { new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy() };
            var source2Branch1Builds = new List<IBuildNode> { new BuildNodeDummy(), new BuildNodeDummy(), new BuildNodeDummy() };

            var sources = new List<SourceGroupNodeDummy> { new SourceGroupNodeDummy(), new SourceGroupNodeDummy() };

            var source1Branch1 = new BranchGroupNodeDummy("Source 1 Branch 1");
            var source1Branch2 = new BranchGroupNodeDummy("Source 1 Branch 2");
            var source2Branch1 = new BranchGroupNodeDummy("Source 2 Branch 1");

            source1Branch1.AddRange(source1Branch1Builds);
            source1Branch2.AddRange(source1Branch2Builds);
            source2Branch1.AddRange(source2Branch1Builds);

            sources[0].AddChild(source1Branch1);
            sources[0].AddChild(source1Branch2);
            sources[1].AddChild(source2Branch1);

            var tree = new BuildTreeDummy(new GroupDefinitionDummy {GroupDefinition.Source, GroupDefinition.Branch});
            tree.AddRange(sources);

            return tree;
        }
    }
}
