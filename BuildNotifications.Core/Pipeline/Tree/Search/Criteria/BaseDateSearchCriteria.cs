using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BuildNotifications.Core.Config;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
    public abstract class BaseDateSearchCriteria : BaseSearchCriteria
    {
        private Func<DateTime> _resolveNow;

        protected DateTime Today() => _resolveNow().Date;

        protected const int MaxDatesToSuggest = 3;

        protected BaseDateSearchCriteria(IPipeline pipeline) : base(pipeline)
        {
            _resolveNow = () => DateTime.Now;
        }

        private static readonly SuggestionTupleEqualByDateTime SuggestionTupleEqualByDateTimeEqualityComparer = new SuggestionTupleEqualByDateTime();

        internal void SetResolveNow(Func<DateTime> to) => _resolveNow = to;

        protected IEnumerable<string> SuggestInputWithToday(string inputSoFar)
        {
            var todayInLocalizedString = DateTime.Today.ToString("d", CurrentCultureInfo);

            if (string.IsNullOrWhiteSpace(inputSoFar))
                yield break;

            var suggestion = new StringBuilder(inputSoFar);
            var charactersToAddFromTodayString = 0;

            // add as little characters from the today date as possible to potentially complete the input to a valid date. E.g. input "01/0" is a complete date when adding "3/2020" which are the last 6 characters of the assumed today date "01/03/2020"
            do
            {
                if (DateTime.TryParse(suggestion.ToString(), CurrentCultureInfo, DateTimeStyles.AssumeLocal, out var asDateTime) && asDateTime.Year == DateTime.Today.Year)
                    yield return suggestion.ToString();

                charactersToAddFromTodayString += 1;

                suggestion.Clear();
                suggestion.Append(inputSoFar);
                suggestion.Append(todayInLocalizedString.Substring(todayInLocalizedString.Length - charactersToAddFromTodayString, charactersToAddFromTodayString));
            } while (charactersToAddFromTodayString < todayInLocalizedString.Length);
        }

        protected IEnumerable<string> ParseSuggestions(IEnumerable<string> suggestions)
        {
            return suggestions
                .Select(AsDateTimeTuple)
                .Distinct(SuggestionTupleEqualByDateTimeEqualityComparer) // only unique dates
                .Select(t => t.suggestion);
        }

        protected override IEnumerable<string> SuggestInternal(string input, StringMatcher stringMatcher) => ParseSuggestions(SuggestDatesInternal(input, stringMatcher));

        protected abstract IEnumerable<string> SuggestDatesInternal(string input, StringMatcher stringMatcher);

        private (DateTime dateTime, string suggestion) AsDateTimeTuple(string dateTimeAsString)
        {
            if (DateTime.TryParse(dateTimeAsString, CurrentCultureInfo, DateTimeStyles.AssumeLocal, out var asDateTime))
                return (asDateTime, dateTimeAsString);

            // use hashcode of string to get unique DateTimes.
            return (DateTime.Now, dateTimeAsString);
        }

        private readonly StringMatcher _suggestionStringMatcher = new StringMatcher();

        protected IEnumerable<string> SuggestPossibleDates(string inputSoFar, IEnumerable<DateTime> possibleDates)
        {
            foreach (var possibleDate in possibleDates)
            {
                var possibleDateAsString = possibleDate.ToString("d", CurrentCultureInfo);
                if (inputSoFar.Length > possibleDateAsString.Length)
                    continue;

                var inputAutoFilledWithPossibleDate = inputSoFar + possibleDateAsString.Substring(inputSoFar.Length, possibleDateAsString.Length - inputSoFar.Length);
                if (DateTime.TryParse(inputAutoFilledWithPossibleDate, CurrentCultureInfo, DateTimeStyles.AssumeLocal, out var asDateTime) && asDateTime.Equals(possibleDate))
                    yield return inputAutoFilledWithPossibleDate;
                else
                {
                    _suggestionStringMatcher.SearchPattern = inputSoFar;
                    if (_suggestionStringMatcher.IsMatch(possibleDateAsString))
                        yield return possibleDateAsString;
                }
            }
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