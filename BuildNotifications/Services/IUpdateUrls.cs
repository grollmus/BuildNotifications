using System;

namespace BuildNotifications.Services;

internal interface IUpdateUrls
{
    Uri BaseAddressForApiRequests();
    Uri BaseAddressOf(Uri url);
    Uri DownloadFileFromReleasePackage(string updateUrl, string fileName);
    Uri ListReleases();
    Uri RelativeFileDownloadUrl(Uri url);
}