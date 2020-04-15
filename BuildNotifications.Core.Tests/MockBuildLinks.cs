using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Tests
{
    internal class MockBuildLinks : IBuildLinks
    {
        public string? BuildWeb { get; } = null;
        public string? BranchWeb { get; } = null;
        public string? DefinitionWeb { get; } = null;

        public void UpdateWith(IBranch branch)
        {
            // Do nothing
        }
    }
}