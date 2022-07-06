using System.Globalization;
using BuildNotifications.PluginInterfaces.SourceControl;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace BuildNotifications.Plugin.Tfs.SourceControl;

internal class TfsPullRequests : TfsBranch, IPullRequest
{
    public TfsPullRequests(GitPullRequest native, TfsUrlBuilder urlBuilder)
        : base(native.PullRequestId, urlBuilder)
    {
        Description = native.Description;
        SourceBranch = native.SourceRefName;
        TargetBranch = native.TargetRefName;
        Id = native.PullRequestId.ToString(CultureInfo.InvariantCulture);
    }

    public override bool IsPullRequest => true;
    public string Description { get; }
    public string Id { get; }
    public string SourceBranch { get; }
    public string TargetBranch { get; }
}