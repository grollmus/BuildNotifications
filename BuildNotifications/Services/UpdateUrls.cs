using System;
using System.Text;

namespace BuildNotifications.Services
{
    internal class UpdateUrls : IUpdateUrls
    {
        public Uri BaseAddressForApiRequests()
        {
            return new Uri(GitHubApiUrl, UriKind.Absolute);
        }

        public Uri BaseAddressOf(Uri url)
        {
            return new Uri(url.Scheme + "://" + url.Host, UriKind.Absolute);
        }

        public Uri DownloadFileFromReleasePackage(string updateUrl, string fileName)
        {
            return new Uri(updateUrl.TrimEnd('/') + "/" + fileName.TrimStart('/'), UriKind.Absolute);
        }

        public Uri ListReleases()
        {
            var repoUri = new Uri(UpdateUrl, UriKind.Absolute);
            var releasesApiBuilder = new StringBuilder("repos")
                .Append(repoUri.AbsolutePath)
                .Append("/releases");

            var requestUri = new Uri(releasesApiBuilder.ToString(), UriKind.Relative);
            return requestUri;
        }

        public Uri RelativeFileDownloadUrl(Uri url)
        {
            return new Uri(url.AbsolutePath.TrimStart('/'), UriKind.Relative);
        }

        private const string GitHubApiUrl = "https://api.github.com/";
        private const string UpdateUrl = "https://github.com/grollmus/BuildNotifications";
    }
}