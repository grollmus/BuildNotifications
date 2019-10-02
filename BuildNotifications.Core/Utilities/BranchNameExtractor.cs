using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Utilities
{
    internal class BranchNameExtractor : IBranchNameExtractor
    {
        public string ExtractDisplayName(string fullBranchName)
        {
            if (fullBranchName.StartsWith(GitRefHeadPrefix))
                return fullBranchName.Substring(GitRefHeadPrefix.Length);

            var match = PullRequestPattern.Match(fullBranchName);
            if (match.Success)
                return $"PR {match.Groups[1].Value}";

            return fullBranchName;
        }

        public string ExtractDisplayName(string fullBranchName, IEnumerable<IBranch> allBranches)
        {
            var matchingBranch = allBranches.FirstOrDefault(b => b.Name == fullBranchName);
            return matchingBranch?.DisplayName ?? ExtractDisplayName(fullBranchName);
        }

        private const string GitRefHeadPrefix = "refs/heads/";
        private static readonly Regex PullRequestPattern = new Regex("refs\\/pull\\/([\\d]+)\\/merge", RegexOptions.Compiled);
    }
}