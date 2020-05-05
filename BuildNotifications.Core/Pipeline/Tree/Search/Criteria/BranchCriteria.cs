using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
    internal class BranchCriteria : BaseSearchCriteria
    {
        public BranchCriteria(IPipeline pipeline) : base(StringLocalizer.SearchCriteriaBranchKeyword, StringLocalizer.SearchCriteriaBranchDescription, pipeline)
        {
        }

        private readonly StringComparer _stringComparer = StringComparer.FromComparison(StringComparison.InvariantCultureIgnoreCase);

        protected override IEnumerable<string> SuggestInternal(string input, StringMatcher stringMatcher)
        {
            return _validBranches.Where(stringMatcher.IsMatch).OrderBy(k => _stringComparer.Compare(input, k));
        }

        private readonly HashSet<string> _validBranches = new HashSet<string>();

        protected override void UpdateCacheForSuggestions(IPipeline pipeline)
        {
            _validBranches.Clear();
            foreach (var branchName in pipeline.CachedBranches().Select(b => b.DisplayName).Distinct())
            {
                _validBranches.Add(branchName);
            }
        }

        private readonly StringMatcher _stringMatcher = new StringMatcher();

        protected override bool IsBuildIncludedInternal(IBuild build, string input)
        {
            if (!_stringMatcher.SearchPattern.Equals(input, StringComparison.InvariantCulture))
                _stringMatcher.SearchPattern = input;

            return _stringMatcher.IsMatch(build.BranchName);
        }

        protected override IEnumerable<string> Examples()
        {
            yield return "stage*";
            yield return "=Stage";
            yield return "*";
            yield return "sta";
        }
    }
}