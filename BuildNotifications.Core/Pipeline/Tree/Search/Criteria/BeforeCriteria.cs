using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
    internal class BeforeCriteria : BaseDateSearchCriteria
    {
        public BeforeCriteria(IPipeline pipeline)
            : base(pipeline)
        {
        }

        private readonly List<DateTime> _validDates = new List<DateTime>();

        protected override IEnumerable<string> SuggestDatesInternal(string input, StringMatcher stringMatcher)
        {
            var todayString = StringLocalizer.SearchCriteriaBeforeToday;
            if (stringMatcher.IsMatch(todayString))
                yield return todayString;

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

        public override string LocalizedKeyword(CultureInfo forCultureInfo) => StringLocalizer.SearchCriteriaBeforeKeyword;

        public override string LocalizedDescription(CultureInfo forCultureInfo) => StringLocalizer.SearchCriteriaBeforeDescription;

        protected override void UpdateCacheForSuggestions(IPipeline pipeline)
        {
            _validDates.Clear();
            _validDates.AddRange(
                pipeline.CachedBuilds()
                    .Where(b => b.QueueTime != null)
                    .Select(b => (DateTime) b.QueueTime!)
                    .Select(d => d.Date + TimeSpan.FromDays(1)) // this criteria checks for builds before the given date. Therefore a valid value for this build would be the day after
                    .Distinct()
                    .Take(MaxDatesToSuggest));
        }

        protected override bool IsBuildIncludedInternal(IBuild build, string input)
        {
            var buildDate = build.QueueTime;
            if (buildDate == null)
                return true;

            if (input.Equals(StringLocalizer.SearchCriteriaBeforeToday, StringComparison.InvariantCultureIgnoreCase))
                return buildDate.Value.Date < Today();

            if (DateTime.TryParse(input, CurrentCultureInfo, DateTimeStyles.AssumeLocal, out var inputAsDateTime))
                return buildDate.Value.Date < inputAsDateTime.Date;

            return false;
        }

        protected override IEnumerable<string> Examples()
        {
            yield return StringLocalizer.SearchCriteriaBeforeToday;
            yield return Today().ToString("d", CurrentCultureInfo);
            yield return (Today() + TimeSpan.FromDays(1)).ToString("d", CurrentCultureInfo);
        }
    }
}