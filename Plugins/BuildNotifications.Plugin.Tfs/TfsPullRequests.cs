using BuildNotifications.PluginInterfaces.SourceControl;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsPullRequests : TfsBranch, IPullRequest
    {
        public TfsPullRequests(GitPullRequest native)
            : base(native.PullRequestId)
        {
            Description = native.Description;
            SourceBranch = native.SourceRefName;
            TargetBranch = native.TargetRefName;
            Id = native.PullRequestId.ToString();
        }

        public string Description { get; }
        public string Id { get; }
        public string SourceBranch { get; }
        public string TargetBranch { get; }
    }
}