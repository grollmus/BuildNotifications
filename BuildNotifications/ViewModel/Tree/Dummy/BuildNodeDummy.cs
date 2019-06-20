using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.ViewModel.Tree.Dummy
{
    internal class BuildNodeDummy : BuildTreeNodeDummy, IBuildNode
    {
        public IBuild Build { get; }
    }
}