using System;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Plugin.DummyServer
{
    public class Build : IBaseBuild
    {
        public Build()
        {
            Id = (++_idCounter).ToString();
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return Id.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        public string BranchName { get; set; }

        public IBuildDefinition Definition { get; set; }

        public string Id { get; set; }

        public DateTime? LastChangedTime { get; set; }

        public int Progress { get; set; }

        public DateTime? QueueTime { get; set; }

        public IUser RequestedBy { get; set; }

        public IUser RequestedFor { get; set; }

        public BuildStatus Status { get; set; }

        public BuildReason Reason { get; set; }

        public IBuildLinks Links { get; } = new DummyBuildLinks();

        public bool Equals(IBaseBuild other)
        {
            return Id == (other as Build)?.Id;
        }

        private static int _idCounter;
    }
}