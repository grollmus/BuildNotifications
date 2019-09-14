using System;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Tests
{
    internal class MockBuild : IBuild
    {
        public MockBuild()
        {
            Id = string.Empty;
            BranchName = string.Empty;
            ProjectName = string.Empty;
            Definition = new MockBuildDefinition();

            RequestedBy = new MockUser();
            RequestedFor = new MockUser();

            Progress = 0;
            QueueTime = null;
            LastChangedTime = null;
            Status = BuildStatus.None;
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
            return mock?.Id.Equals(Id) == true;
        }

        public string BranchName { get; }
        public IBuildDefinition Definition { get; }
        public string Id { get; }
        public DateTime? LastChangedTime { get; }
        public int Progress { get; }
        public DateTime? QueueTime { get; }
        public IUser RequestedBy { get; }
        public IUser RequestedFor { get; }
        public BuildStatus Status { get; }
        public string ProjectName { get; }
    }
}