using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Sight;

namespace BuildNotifications.Core.Pipeline.Tree.Sight
{
    /// <summary>
    /// Highlights all builds which were requested by the current user.
    /// </summary>
    public class MyBuildsSight : ISight
    {
        public bool IsEnabled { get; set; }

        public bool IsHighlighted(IBuild build) => build.IsRequestedByCurrentUser;

        public bool IsBuildShown(IBuild build) => true;
    }
}
