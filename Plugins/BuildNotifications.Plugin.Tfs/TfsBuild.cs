using System;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using Microsoft.TeamFoundation.Build.WebApi;
using BuildStatus = BuildNotifications.PluginInterfaces.Builds.BuildStatus;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsBuild : IBaseBuild
    {
        public TfsBuild(Build build)
        {
            _id = build.Url;
            Id = build.Id.ToString();

            QueueTime = build.QueueTime;
            LastChangedTime = build.QueueTime;
            BranchName = build.SourceBranch;

            _nativeResult = build.Result;
            _nativeStatus = build.Status;

            RequestedBy = new TfsUser(build.RequestedBy);
            RequestedFor = new TfsUser(build.RequestedFor);
            Definition = new TfsBuildDefinition(build.Definition);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_id, Id);
        }

        public bool Equals(IBaseBuild other)
        {
            return _id == (other as TfsBuild)?._id;
        }

        public string BranchName { get; }

        public IBuildDefinition Definition { get; }

        public string Id { get; }

        public DateTime? LastChangedTime { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty
        // TODO: #30
        public int Progress { get; }

        public DateTime? QueueTime { get; }

        public IUser RequestedBy { get; }

        public IUser? RequestedFor { get; }

        public BuildStatus Status
        {
            get
            {
                if (!_nativeStatus.HasValue)
                    return BuildStatus.None;

                switch (_nativeStatus.Value)
                {
                    case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Cancelling:
                        return BuildStatus.Cancelled;

                    case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.InProgress:
                    case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.NotStarted:
                    case Microsoft.TeamFoundation.Build.WebApi.BuildStatus.Postponed:
                        return BuildStatus.Pending;
                }

                if (!_nativeResult.HasValue)
                    return BuildStatus.Pending;

                switch (_nativeResult.Value)
                {
                    case BuildResult.Canceled:
                        return BuildStatus.Cancelled;
                    case BuildResult.Failed:
                        return BuildStatus.Failed;
                    case BuildResult.PartiallySucceeded:
                        return BuildStatus.PartiallySucceeded;
                    case BuildResult.Succeeded:
                        return BuildStatus.Succeeded;
                }

                return BuildStatus.None;
            }
        }

        private readonly string _id;
        private readonly Microsoft.TeamFoundation.Build.WebApi.BuildStatus? _nativeStatus;
        private readonly BuildResult? _nativeResult;
    }
}