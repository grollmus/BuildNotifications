using BuildNotifications.PluginInterfaces.SourceControl;
using Microsoft.TeamFoundation.SourceControl.WebApi;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsBranch : IBranch
    {
        public TfsBranch(GitRef branch)
        {
            DisplayName = ExtractDisplayName(branch.Name);
            Name = branch.Name;
            _id = branch.ObjectId;
        }

        protected TfsBranch(int pullRequestId)
        {
            DisplayName = $"PR {pullRequestId}";
            Name = ComputePullRequestBranchName(pullRequestId);
            _id = pullRequestId.ToString();
        }

        internal static string ComputePullRequestBranchName(int pullRequestId)
        {
            return PullRequestPrefix + pullRequestId + PullRequestSuffix;
        }

        private string ExtractDisplayName(string branchName)
        {
            return branchName.Replace(BranchNamePrefix, "");
        }

        public bool Equals(IBranch other)
        {
            return _id == (other as TfsBranch)?._id;
        }

        public string DisplayName { get; }

        public string Name { get; }

        private readonly string _id;
        private const string BranchNamePrefix = "refs/heads/";
        private const string PullRequestPrefix = "refs/pull/";
        private const string PullRequestSuffix = "/merge";
    }
}