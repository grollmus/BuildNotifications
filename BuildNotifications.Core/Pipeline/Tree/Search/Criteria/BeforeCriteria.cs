using System;
using System.Collections.Generic;
using System.Globalization;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
    internal class BeforeCriteria : BaseDateSearchCriteria
    {
        public BeforeCriteria() : base(StringLocalizer.SearchCriteriaBeforeKeyword, StringLocalizer.SearchCriteriaBeforeDescription)
        {
        }

        private readonly string _todayString = StringLocalizer.SearchCriteriaBeforeToday;

        protected override IEnumerable<string> SuggestInternal(string input, StringMatcher stringMatcher)
        {
            if (stringMatcher.IsMatch(_todayString))
                yield return _todayString;

            var suggestions = SuggestInputWithToday(input);
            foreach (var suggestion in suggestions)
            {
                yield return suggestion;
            }
        }

        protected override bool IsBuildIncludedInternal(IBuild build, string input)
        {
            var buildDate = build.QueueTime;
            if (buildDate == null)
                return true;

            if (input.Equals(_todayString, StringComparison.InvariantCultureIgnoreCase))
                return buildDate.Value.Date < DateTime.Today;

            if (DateTime.TryParse(input, CurrentCultureInfo, DateTimeStyles.AssumeLocal, out var inputAsDateTime))
                return buildDate.Value.Date < inputAsDateTime.Date;

            return false;
        }

        protected override IEnumerable<string> Examples()
        {
            yield return _todayString;
            yield return DateTime.Today.ToString("d", CurrentCultureInfo);
            yield return (DateTime.Today - TimeSpan.FromDays(1)).ToString("d", CurrentCultureInfo);
        }
    }
}