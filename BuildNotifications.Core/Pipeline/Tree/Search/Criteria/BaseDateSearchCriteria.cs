using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
    internal abstract class BaseDateSearchCriteria : BaseSearchCriteria
    {
        protected BaseDateSearchCriteria(string localizedKeyword, string localizedDescription) : base(localizedKeyword, localizedDescription)
        {
        }

        private static readonly SuggestionTupleEqualByDateTime SuggestionTupleEqualByDateTimeEqualityComparer = new SuggestionTupleEqualByDateTime();

        protected IEnumerable<string> SuggestInputWithToday(string inputSoFar)
        {
            var todayInLocalizedString = DateTime.Now.ToString("d", CurrentCultureInfo);
            var suggestions = new List<(DateTime dateTime, string suggestion)>();

            if (string.IsNullOrWhiteSpace(inputSoFar))
                return todayInLocalizedString.Yield();

            var suggestion = new StringBuilder(inputSoFar);
            var charactersToAddFromTodayString = 0;

            // add as little characters from the today date as possible to potentially complete the input to a valid date. E.g. input "01/0" is a complete date when adding "3/2020" which are the last 6 characters of the assumed today date "01/03/2020"
            do
            {
                if (DateTime.TryParse(suggestion.ToString(), CurrentCultureInfo, DateTimeStyles.AssumeLocal, out var asDateTime) && asDateTime.Year == DateTime.Today.Year)
                    suggestions.Add((asDateTime, suggestion.ToString()));

                charactersToAddFromTodayString += 1;

                suggestion.Clear();
                suggestion.Append(inputSoFar);
                suggestion.Append(todayInLocalizedString.Substring(todayInLocalizedString.Length - charactersToAddFromTodayString, charactersToAddFromTodayString));
            } while (charactersToAddFromTodayString < todayInLocalizedString.Length);

            return suggestions
                .OrderBy(t => t.suggestion.Length) // shortest to longest
                .Distinct(SuggestionTupleEqualByDateTimeEqualityComparer) // only unique dates
                .OrderByDescending(t => t.dateTime) // display highest dates first
                .Select(t => t.suggestion);
        }

        private class SuggestionTupleEqualByDateTime : EqualityComparer<(DateTime dateTime, string suggestion)>
        {
            public override bool Equals((DateTime dateTime, string suggestion) x, (DateTime dateTime, string suggestion) y)
            {
                return x.dateTime.Equals(y.dateTime);
            }

            public override int GetHashCode((DateTime dateTime, string suggestion) tuple)
            {
                return tuple.dateTime.GetHashCode();
            }
        }
    }
}