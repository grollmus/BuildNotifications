using System;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Sight;

namespace BuildNotifications.Core.Pipeline.Tree.Sight
{
    /// <summary>
    /// Filters all non manual builds.
    /// </summary>
    public class ManualBuildsSight : ISight
    {
        public bool IsEnabled { get; set; }

        public bool IsHighlighted(IBuild build) => false;

        public bool IsBuildShown(IBuild build) => build.RequestedBy.Id.Equals(build.RequestedFor?.Id, StringComparison.InvariantCulture);
    }
}
