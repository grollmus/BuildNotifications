using System;
using System.Globalization;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using Microsoft.TeamFoundation.Build.WebApi;
using BuildReason = BuildNotifications.PluginInterfaces.Builds.BuildReason;
using BuildStatus = BuildNotifications.PluginInterfaces.Builds.BuildStatus;

namespace BuildNotifications.Plugin.Tfs.Build;

internal class TfsBuild : IBaseBuild
{
    public TfsBuild(Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
        _id = build.Url;
        BuildId = build.Id;
        Id = build.Id.ToString(CultureInfo.InvariantCulture);

        QueueTime = build.QueueTime;
        LastChangedTime = build.QueueTime;
        BranchName = build.SourceBranch;

        _nativeResult = build.Result;
        _nativeStatus = build.Status;
        _nativeReason = build.Reason;

        RequestedBy = new TfsUser(build.RequestedBy);
        RequestedFor = new TfsUser(build.RequestedFor);
        Definition = new TfsBuildDefinition(build.Definition);
        DisplayName = build.BuildNumber;

        Links = new TfsLinks(build);
    }

    internal int BuildId { get; }

    public override bool Equals(object? obj) => Equals(obj as IBaseBuild);

    public override int GetHashCode() => HashCode.Combine(_id, Id);

    public bool Equals(IBaseBuild? other) => _id == (other as TfsBuild)?._id;

    public string BranchName { get; }

    public IBuildDefinition Definition { get; }

    public string Id { get; }

    public DateTime? LastChangedTime { get; }

    public int Progress { get; internal set; }

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
                    return BuildStatus.Running;
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

    public string DisplayName { get; }

    public BuildReason Reason
    {
        get
        {
            switch (_nativeReason)
            {
                case Microsoft.TeamFoundation.Build.WebApi.BuildReason.None:
                    return BuildReason.Unknown;
                case Microsoft.TeamFoundation.Build.WebApi.BuildReason.Manual:
                case Microsoft.TeamFoundation.Build.WebApi.BuildReason.UserCreated:
                    return BuildReason.Manual;
                case Microsoft.TeamFoundation.Build.WebApi.BuildReason.IndividualCI:
                case Microsoft.TeamFoundation.Build.WebApi.BuildReason.BatchedCI:
                case Microsoft.TeamFoundation.Build.WebApi.BuildReason.ValidateShelveset:
                case Microsoft.TeamFoundation.Build.WebApi.BuildReason.CheckInShelveset:
                    return BuildReason.CheckedIn;
                case Microsoft.TeamFoundation.Build.WebApi.BuildReason.Schedule:
                case Microsoft.TeamFoundation.Build.WebApi.BuildReason.ScheduleForced:
                    return BuildReason.Scheduled;
                case Microsoft.TeamFoundation.Build.WebApi.BuildReason.PullRequest:
                    return BuildReason.PullRequest;
                default:
                    return BuildReason.Other;
            }
        }
    }

    public IBuildLinks Links { get; }

    private readonly string _id;
    private readonly Microsoft.TeamFoundation.Build.WebApi.BuildStatus? _nativeStatus;
    private readonly BuildResult? _nativeResult;
    private readonly Microsoft.TeamFoundation.Build.WebApi.BuildReason _nativeReason;
}