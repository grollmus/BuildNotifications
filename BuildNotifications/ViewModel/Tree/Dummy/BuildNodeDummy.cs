using System;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.ViewModel.Tree.Dummy
{
    internal class BuildNodeDummy : BuildTreeNodeDummy, IBuildNode
    {
        public BuildNodeDummy(BuildStatus initialStatus = BuildStatus.Succeeded)
        {
            Build = new BuildDummy(initialStatus);
        }

        public IBuild Build { get; }
    }

    internal class BuildDummy : IBuild
    {
        public BuildDummy(BuildStatus initialStatus)
        {
            Status = initialStatus;
        }

        public string BranchName { get; }
        public IBuildDefinition Definition { get; }
        public string Id { get; }
        public DateTime? LastChangedTime { get; }
        public int Progress { get; }
        public DateTime? QueueTime { get; }
        public IUser RequestedBy { get; }
        public IUser RequestedFor { get; }
        public BuildStatus Status { get; set; }

        public bool Equals(IBaseBuild other)
        {
            throw new NotImplementedException();
        }

        public string ProjectName { get; }
        private static Random _rnd = new Random();
    }
}