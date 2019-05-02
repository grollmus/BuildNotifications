using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal interface IBranchGroupNode : IGroupNode
    {
        IBranch Branch { get; }
    }
}