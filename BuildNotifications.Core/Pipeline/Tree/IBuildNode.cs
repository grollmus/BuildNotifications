using System;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree
{
    public interface IBuildNode : IBuildTreeNode
    {
        IBuild Build { get; }
        DateTime? LastChangedTime { get; }
        DateTime? QueueTime { get; }
        int Progress { get; }
        BuildStatus Status { get; }
    }
}