using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree
{
    public interface IBuildNode : IBuildTreeNode
    {
        IBuild Build { get; }
    }
}