using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
    internal class DuringCriteria : BaseDateSearchCriteria
    {
        public DuringCriteria(IPipeline pipeline) : base(pipeline)
        {
        }

        private readonly string _todayString = StringLocalizer.SearchCriteriaDuringToday;
        private readonly string _yesterdayString = StringLocalizer.SearchCriteriaDuringYesterday;

        private readonly List<DateTime> _validDates = new List<DateTime>();

        protected override IEnumerable<string> SuggestDatesInternal(string input, StringMatcher stringMatcher)
        {
            if (input.StartsWith("y", StringComparison.CurrentCultureIgnoreCase))
            {
                if (stringMatcher.IsMatch(_yesterdayString))
                    yield return _yesterdayString;

                if (stringMatcher.IsMatch(_todayString))
                    yield return _todayString;
            }
            else
            {
                if (stringMatcher.IsMatch(_todayString))
                    yield return _todayString;

                if (stringMatcher.IsMatch(_yesterdayString))
                    yield return _yesterdayString;
            }

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

        public override string LocalizedKeyword(CultureInfo forCultureInfo) => StringLocalizer.SearchCriteriaDuringKeyword;

        public override string LocalizedDescription(CultureInfo forCultureInfo) => StringLocalizer.SearchCriteriaDuringDescription;

        protected override void UpdateCacheForSuggestions(IPipeline pipeline)
        {
            _validDates.Clear();
            _validDates.AddRange(
                pipeline.CachedBuilds()
                    .Where(b => b.QueueTime != null)
                    .Select(b => (DateTime) b.QueueTime!)
                    .Select(d => d.Date)
                    .Distinct()
                    .Take(MaxDatesToSuggest));
        }

        protected override bool IsBuildIncludedInternal(IBuild build, string input)
        {
            var buildDate = build.QueueTime;
            if (buildDate == null)
                return true;

            if (input.Equals(_todayString, StringComparison.InvariantCultureIgnoreCase))
                return buildDate.Value.Date.Equals(DateTime.Today);

            if (input.Equals(_yesterdayString, StringComparison.InvariantCultureIgnoreCase))
                return buildDate.Value.Date.Equals(DateTime.Today - TimeSpan.FromDays(1));

            if (DateTime.TryParse(input, CurrentCultureInfo, DateTimeStyles.AssumeLocal, out var inputAsDateTime))
                return buildDate.Value.Date.Equals(inputAsDateTime.Date);

            return false;
        }

        protected override IEnumerable<string> Examples()
        {
            yield return _todayString;
            yield return _yesterdayString;
            yield return DateTime.Today.ToString("d", CurrentCultureInfo);
        }
    }
}