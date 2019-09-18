using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Tests
{
    internal class MockBuildLinks : IBuildLinks
    {
        public string? BuildWeb { get; } = null;
        public string? BranchWeb { get; } = null;
        public string? DefinitionWeb { get; } = null;
    }
}