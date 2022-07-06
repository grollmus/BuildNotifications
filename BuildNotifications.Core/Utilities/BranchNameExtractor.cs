using System;
using System.Text.RegularExpressions;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Utilities;

internal class BranchNameExtractor : IBranchNameExtractor
{
    public string ExtractDisplayName(string fullBranchName)
    {
        if (fullBranchName.StartsWith(GitRefHeadPrefix, StringComparison.InvariantCulture))
            return fullBranchName.Substring(GitRefHeadPrefix.Length);

        var match = PullRequestPattern.Match(fullBranchName);
        if (match.Success)
            return $"PR {match.Groups[1].Value}";

        return fullBranchName;
    }

    private const string GitRefHeadPrefix = "refs/heads/";
    private static readonly Regex PullRequestPattern = new("refs\\/pull\\/([\\d]+)\\/merge", RegexOptions.Compiled);
}