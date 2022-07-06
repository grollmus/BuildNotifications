using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria;

internal class DuringCriteria : BaseDateSearchCriteria
{
    public DuringCriteria(IPipeline pipeline)
        : base(pipeline)
    {
    }

    public override string LocalizedDescription(CultureInfo forCultureInfo) => StringLocalizer.SearchCriteriaDuringDescription;

    public override string LocalizedKeyword(CultureInfo forCultureInfo) => StringLocalizer.SearchCriteriaDuringKeyword;

    protected override IEnumerable<string> Examples()
    {
        yield return StringLocalizer.SearchCriteriaDuringToday;
        yield return StringLocalizer.SearchCriteriaDuringYesterday;
        yield return Today().ToString("d", CurrentCultureInfo);
    }

    protected override bool IsBuildIncludedInternal(IBuild build, string input)
    {
        var buildDate = build.QueueTime;
        if (buildDate == null)
            return true;

        if (input.Equals(StringLocalizer.SearchCriteriaDuringToday, StringComparison.InvariantCultureIgnoreCase))
            return buildDate.Value.Date.Equals(Today());

        if (input.Equals(StringLocalizer.SearchCriteriaDuringYesterday, StringComparison.InvariantCultureIgnoreCase))
            return buildDate.Value.Date.Equals(Today() - TimeSpan.FromDays(1));

        if (DateTime.TryParse(input, CurrentCultureInfo, DateTimeStyles.AssumeLocal, out var inputAsDateTime))
            return buildDate.Value.Date.Equals(inputAsDateTime.Date);

        return false;
    }

    protected override IEnumerable<string> SuggestDatesInternal(string input, StringMatcher stringMatcher)
    {
        var todayString = StringLocalizer.SearchCriteriaDuringToday;
        var yesterdayString = StringLocalizer.SearchCriteriaDuringYesterday;
        if (input.StartsWith("y", StringComparison.CurrentCultureIgnoreCase))
        {
            if (stringMatcher.IsMatch(yesterdayString))
                yield return yesterdayString;

            if (stringMatcher.IsMatch(todayString))
                yield return todayString;
        }
        else
        {
            if (stringMatcher.IsMatch(todayString))
                yield return todayString;

            if (stringMatcher.IsMatch(yesterdayString))
                yield return yesterdayString;
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

    protected override void UpdateCacheForSuggestions(IPipeline pipeline)
    {
        _validDates.Clear();
        _validDates.AddRange(
            pipeline.CachedBuilds()
                .Where(b => b.QueueTime != null)
                .Select(b => (DateTime)b.QueueTime!)
                .Select(d => d.Date)
                .Distinct()
                .Take(MaxDatesToSuggest));
    }

    private readonly List<DateTime> _validDates = new();
}