using System;
using BuildNotifications.PluginInterfaces;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline;

internal class EnrichedBuild : IBuild
{
    public EnrichedBuild(IBaseBuild build, string projectName, Guid projectId, IBuildProvider provider)
    {
        OriginalBuild = build;
        ProjectName = projectName;
        ProjectId = projectId;
        Provider = provider;
        BranchName = OriginalBuild.BranchName;
        IsRequestedByCurrentUser = provider.User.Id == build.RequestedBy.Id;
    }

    public IBuildProvider Provider { get; }

    internal IBaseBuild OriginalBuild { get; }

    public IBranch? Branch { get; set; }

    public bool Equals(IBaseBuild other) => OriginalBuild.Equals(other);

    public string ProjectName { get; }

    public Guid ProjectId { get; }

    public bool IsRequestedByCurrentUser { get; }

    public string BranchName { get; set; }

    public IBuildDefinition Definition => OriginalBuild.Definition;

    public string Id => OriginalBuild.Id;

    public DateTime? LastChangedTime => OriginalBuild.LastChangedTime;

    public int Progress => OriginalBuild.Progress;

    public DateTime? QueueTime => OriginalBuild.QueueTime;

    public IUser RequestedBy => OriginalBuild.RequestedBy;

    public IUser? RequestedFor => OriginalBuild.RequestedFor;

    public BuildStatus Status => OriginalBuild.Status;

    public BuildReason Reason => OriginalBuild.Reason;

    public IBuildLinks Links => OriginalBuild.Links;
}