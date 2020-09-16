using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline
{
    internal class BranchMatcher : IBuildMatcher
    {
        public BranchMatcher(IBranchNameExtractor branchNameExtractor, IEnumerable<StringMatcher> matchers)
        {
            _branchNameExtractor = branchNameExtractor;
            _matchers = matchers.ToList();
        }

        public bool IsMatch(IBaseBuild build)
        {
            var branchName = _branchNameExtractor.ExtractDisplayName(build.BranchName);
            return _matchers.Any(m => m.IsMatch(branchName));
        }

        private readonly IBranchNameExtractor _branchNameExtractor;
        private readonly IEnumerable<StringMatcher> _matchers;
    }
}