using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.TestMocks
{
    public class MockBuildLinks : IBuildLinks
    {
        public string? BuildWeb { get; } = null;
        public string? BranchWeb { get; } = null;
        public string? DefinitionWeb { get; } = null;

        public void UpdateWith(IBranch branch)
        {
        }
    }
}