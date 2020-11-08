using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
    internal class IsCriteria : BaseSearchCriteria
    {
        public IsCriteria(IPipeline pipeline)
            : base(pipeline)
        {
        }

        // Status
        private static readonly string Cancelled = StringLocalizer.SearchCriteriaIsCancelled;
        private static readonly string Failed = StringLocalizer.SearchCriteriaIsFailed;
        private static readonly string Succeeded = StringLocalizer.SearchCriteriaIsSucceeded;
        private static readonly string PartiallySucceeded = StringLocalizer.SearchCriteriaIsPartiallySucceeded;
        private static readonly string InProgress = StringLocalizer.SearchCriteriaIsInProgress;
        private static readonly string Pending = StringLocalizer.SearchCriteriaIsPending;

        // Reason
        private static readonly string Manual = StringLocalizer.SearchCriteriaIsManual;
        private static readonly string Ci = StringLocalizer.SearchCriteriaIsCi;
        private static readonly string Scheduled = StringLocalizer.SearchCriteriaIsScheduled;
        private static readonly string PullRequest = StringLocalizer.SearchCriteriaIsPullRequest;

        // Meta
        private static readonly string ByMe = StringLocalizer.SearchCriteriaIsByMe;

        private static IEnumerable<string> AllValueKeywords => LogicTuples.Keys;

        private static IDictionary<string, Func<IBuild, bool>> LogicTuples { get; } = new Dictionary<string, Func<IBuild, bool>>
        {
            {Cancelled, b => b.Status == BuildStatus.Cancelled},
            {Failed, b => b.Status == BuildStatus.Failed},
            {Succeeded, b => b.Status == BuildStatus.Succeeded},
            {PartiallySucceeded, b => b.Status == BuildStatus.PartiallySucceeded},
            {InProgress, b => b.Status == BuildStatus.Running},
            {Pending, b => b.Status == BuildStatus.Pending},
            {Manual, b => b.Reason == BuildReason.Manual},
            {Ci, b => b.Reason == BuildReason.CheckedIn},
            {Scheduled, b => b.Reason == BuildReason.Scheduled},
            {PullRequest, b => b.Reason == BuildReason.PullRequest},
            {ByMe, b => b.IsRequestedByCurrentUser},
        };

        private readonly StringComparer _stringComparer = StringComparer.FromComparison(StringComparison.InvariantCultureIgnoreCase);

        public override string LocalizedKeyword(CultureInfo forCultureInfo) => StringLocalizer.SearchCriteriaIsKeyword;

        public override string LocalizedDescription(CultureInfo forCultureInfo) => StringLocalizer.SearchCriteriaIsDescription;

        protected override IEnumerable<string> SuggestInternal(string input, StringMatcher stringMatcher)
        {
            return AllValueKeywords
                .Where(stringMatcher.IsMatch)
                .OrderBy(k => _stringComparer.Compare(input, k));
        }

        protected override void UpdateCacheForSuggestions(IPipeline pipeline)
        {
        }

        protected override bool IsBuildIncludedInternal(IBuild build, string input)
        {
            var matchingKeywords = AllValueKeywords.Where(k => k.Equals(input, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (matchingKeywords.Count != 1)
                return false;

            var keyword = matchingKeywords[0];
            return LogicTuples[keyword](build);
        }

        protected override IEnumerable<string> Examples()
        {
            yield return Succeeded;
            yield return ByMe;
            yield return Manual;
            yield return PullRequest;
        }
    }
}