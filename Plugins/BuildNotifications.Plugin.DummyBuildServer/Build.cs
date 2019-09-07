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

        /// <inheritdoc />
        public string BranchName { get; set; }

        /// <inheritdoc />
        public IBuildDefinition Definition { get; set; }

        /// <inheritdoc />
        public string Id { get; set; }

        /// <inheritdoc />
        public DateTime? LastChangedTime { get; set; }

        /// <inheritdoc />
        public int Progress { get; set; }

        /// <inheritdoc />
        public DateTime? QueueTime { get; set; }

        /// <inheritdoc />
        public IUser RequestedBy { get; set; }

        /// <inheritdoc />
        public IUser RequestedFor { get; set; }

        /// <inheritdoc />
        public BuildStatus Status { get; set; }

        public bool Equals(IBaseBuild other)
        {
            return Id == (other as Build)?.Id;
        }

        private static int _idCounter;
    }
}