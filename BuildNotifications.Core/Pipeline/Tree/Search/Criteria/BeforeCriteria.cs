using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;

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

        public override bool IsBuildIncluded(IBuild build, string input)
        {
            var buildDate = build.QueueTime;
            if (buildDate == null)
                return true;

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

    public abstract class BaseSearchCriteria : ISearchCriteria
    {
        protected BaseSearchCriteria(string localizedKeyword, string localizedDescription)
        {
            LocalizedKeyword = localizedKeyword;
            LocalizedDescription = localizedDescription;
            _currentCultureInfo = CultureInfo.CurrentUICulture;
        }

        private CultureInfo _currentCultureInfo;

        protected CultureInfo CurrentCultureInfo
        {
            get => _currentCultureInfo;
            private set => _currentCultureInfo = value;
        }

        internal void UseSpecificCulture(CultureInfo cultureInfo)
        {
            _currentCultureInfo = cultureInfo;
        }

        public string LocalizedKeyword { get; }

        public string LocalizedDescription { get; }

        public IEnumerable<ISearchCriteriaSuggestion> Suggest(string input)
        {
            _stringMatcher.SearchPattern = input;
            return SuggestInternal(input, _stringMatcher).Select(AsSuggestion);
        }

        /// <summary>
        /// Create suggestions for the given input so far.
        /// </summary>
        /// <param name="input">The input entered so far.</param>
        /// <param name="stringMatcher">StringMatcher with the input set. To be used to match suggestions.</param>
        /// <returns>Plain text suggestions for the given input.</returns>
        protected abstract IEnumerable<string> SuggestInternal(string input, StringMatcher stringMatcher);

        public abstract bool IsBuildIncluded(IBuild build, string input);

        public IEnumerable<string> LocalizedExamples => Examples();

        protected abstract IEnumerable<string> Examples();

        private readonly StringMatcher _stringMatcher = new StringMatcher();

        protected ISearchCriteriaSuggestion AsSuggestion(string suggestion)
        {
            return new SearchCriteriaSuggestion(suggestion);
        }
    }
}