using System;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.ViewModel.Tree.Dummy
{
    internal class BuildNodeDummy : BuildTreeNodeDummy, IBuildNode
    {
        public IBuild Build { get; }

        public BuildNodeDummy(BuildStatus initialStatus = BuildStatus.Succeeded)
        {
            Build = new BuildDummy(initialStatus);
        }
    }

    internal class BuildDummy : IBuild
    {
        private static Random _rnd = new Random();

        public BuildDummy(BuildStatus initialStatus)
        {
            Status = initialStatus;
        }

        public bool Equals(IBuild other)
        {
            throw new NotImplementedException();
        }

        public string BranchName { get; }
        public IBuildDefinition Definition { get; }
        public string Id { get; }
        public DateTime? LastChangedTime { get; }
        public DateTime? QueueTime { get; }
        public IUser RequestedBy { get; }
        public IUser RequestedFor { get; }
        public BuildStatus Status { get; set; }
    }
}