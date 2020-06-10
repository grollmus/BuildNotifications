using System;
using System.Globalization;
using Microsoft.VisualStudio.Services.Common;

namespace BuildNotifications.Plugin.Tfs
{
    internal class TfsUrlBuilder
    {
        public TfsUrlBuilder(Uri baseUri, string projectName)
        {
            _baseUri = baseUri;
            _projectName = projectName;
        }

        public string BuildBranchUrl(string branchName)
        {
            var builder = new UriBuilder(_baseUri);
            builder.AppendPathSegments("_git", _projectName).AppendQuery("version", $"GB{branchName}");

            return builder.Uri.AbsoluteUri;
        }

        private readonly Uri _baseUri;
        private readonly string _projectName;

        public string BuildPullRequestUrl(int pullRequestId)
        {
            var builder = new UriBuilder(_baseUri);
            builder.AppendPathSegments("_git", _projectName, "pullRequest", pullRequestId.ToString(CultureInfo.InvariantCulture));

            return builder.Uri.AbsoluteUri;
        }
    }
}