using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
    internal class AfterCriteria : BaseDateSearchCriteria
    {
        public AfterCriteria(IPipeline pipeline) : base(StringLocalizer.SearchCriteriaAfterKeyword, StringLocalizer.SearchCriteriaAfterDescription, pipeline)
        {
        }

        private const int MaxDatesToSuggest = 3;

        private readonly string _yesterdayString = StringLocalizer.SearchCriteriaAfterYesterday;
        private readonly List<DateTime> _validDates = new List<DateTime>();

        protected override IEnumerable<string> SuggestDatesInternal(string input, StringMatcher stringMatcher)
        {
            if (stringMatcher.IsMatch(_yesterdayString))
                yield return _yesterdayString;

            var suggestionOfPossibleDates = SuggestPossibleDates(input, _validDates);
            foreach (var suggestionOfPossibleDate in suggestionOfPossibleDates)
            {
                yield return suggestionOfPossibleDate;
            }

            var suggestions = SuggestInputWithToday(input);
            foreach (var suggestion in suggestions)
            {
                yield return suggestion;
            }
        }

        protected override void UpdateCacheForSuggestions(IPipeline pipeline)
        {
            _validDates.Clear();
            _validDates.AddRange(
                pipeline.CachedBuilds()
                    .Where(b => b.QueueTime != null)
                    .Select(b => (DateTime) b.QueueTime!)
                    .Select(d => d.Date - TimeSpan.FromDays(1)) // this criteria checks for builds after the given date. Therefore a valid value for this build would be the day before
                    .Distinct()
                    .Take(MaxDatesToSuggest));
        }

        protected override bool IsBuildIncludedInternal(IBuild build, string input)
        {
            var buildDate = build.QueueTime;
            if (buildDate == null)
                return true;

            if (input.Equals(_yesterdayString, StringComparison.InvariantCultureIgnoreCase))
                return buildDate.Value.Date > (DateTime.Today - TimeSpan.FromDays(1));

            if (DateTime.TryParse(input, CurrentCultureInfo, DateTimeStyles.AssumeLocal, out var inputAsDateTime))
                return buildDate.Value.Date > inputAsDateTime.Date;

            return false;
        }

        protected override IEnumerable<string> Examples()
        {
            yield return _yesterdayString;
            yield return DateTime.Today.ToString("d", CurrentCultureInfo);
            yield return (DateTime.Today - TimeSpan.FromDays(1)).ToString("d", CurrentCultureInfo);
        }
    }
}
