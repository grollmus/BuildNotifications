using System;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Tests
{
    internal class MockBuild : IBuild
    {
        public MockBuild()
        {
        }

        public MockBuild(string id, IBuildDefinition buildDefinition, string branchName)
            : this()
        {
            Id = id;
            Definition = buildDefinition;
            BranchName = branchName;
        }

        public bool Equals(IBaseBuild other)
        {
            var mock = other as MockBuild;
            return mock?.Id.Equals(Id, StringComparison.InvariantCulture) == true;
        }

        public string BranchName { get; } = string.Empty;

        public IBuildDefinition Definition { get; } = new MockBuildDefinition();

        public string Id { get; } = string.Empty;

        public DateTime? LastChangedTime { get; } = null;

        public int Progress { get; } = 0;

        public DateTime? QueueTime { get; } = null;

        public IUser RequestedBy { get; } = new MockUser();

        public IUser RequestedFor { get; } = new MockUser();

        public BuildStatus Status { get; } = BuildStatus.None;

        public BuildReason Reason { get; } = BuildReason.Other;

        public IBuildLinks Links { get; } = new MockBuildLinks();

        public string ProjectName { get; } = string.Empty;

        public Guid ProjectId { get; } = Guid.Empty;

        public bool IsRequestedByCurrentUser { get; } = false;
    }
}