using System;
using System.Threading.Tasks;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Plugin.DummyBuildServer
{
    internal class Build : IBaseBuild
    {
        public Build()
        {
            Id = (++_idCounter).ToString();
            StartProgress();
        }

        public override int GetHashCode()
        {
            // ReSharper disable NonReadonlyMemberInGetHashCode
            return Id.GetHashCode();
            // ReSharper restore NonReadonlyMemberInGetHashCode
        }

        private async void StartProgress()
        {
            while ((Status == BuildStatus.Running || Status == BuildStatus.Pending || Status == BuildStatus.None) && Progress < 100)
            {
                await Task.Delay(100);
                if (Status == BuildStatus.Pending)
                    continue;
                Progress += 1;
                Progress = Progress;
                if (Progress >= 100)
                {
                    Progress = 100;
                    Status = BuildStatus.Succeeded;
                }
            }
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