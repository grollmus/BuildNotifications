using BuildNotifications.PluginInterfaces.SourceControl;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsBranch : IBranch
    {
        public TfsBranch(GitRef branch, TfsUrlBuilder urlBuilder)
        {
            DisplayName = ExtractDisplayName(branch.Name);
            FullName = branch.Name;
            _id = branch.ObjectId;

            WebUrl = urlBuilder.BuildBranchUrl(DisplayName);
        }

        protected TfsBranch(int pullRequestId, TfsUrlBuilder urlBuilder)
        {
            DisplayName = $"PR {pullRequestId}";
            FullName = ComputePullRequestBranchName(pullRequestId);
            _id = pullRequestId.ToString();

            WebUrl = urlBuilder.BuildPullRequestUrl(pullRequestId);
        }

        public string WebUrl { get; }

        internal static string ComputePullRequestBranchName(int pullRequestId) => PullRequestPrefix + pullRequestId + PullRequestSuffix;

        private string ExtractDisplayName(string branchName) => branchName.Replace(BranchNamePrefix, "");

        public bool Equals(IBranch other) => _id == (other as TfsBranch)?._id;

        public string DisplayName { get; }

        public string FullName { get; }
        public virtual bool IsPullRequest => false;

        private readonly string _id;
        private const string BranchNamePrefix = "refs/heads/";
        private const string PullRequestPrefix = "refs/pull/";
        private const string PullRequestSuffix = "/merge";
    }
}