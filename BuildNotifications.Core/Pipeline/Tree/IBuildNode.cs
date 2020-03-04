using System;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree
{
    public interface IBuildNode : IBuildTreeNode
    {
        IBuild Build { get; }
        DateTime? LastChangedTime { get; }
        int Progress { get; }
        DateTime? QueueTime { get; }
        BuildStatus Status { get; }
        bool IsHighlightedBySight { get; set; }
    }
}