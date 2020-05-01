using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
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

        public bool IsBuildIncluded(IBuild build, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            return IsBuildIncludedInternal(build, input.TrimStart(' ').TrimEnd(' '));
        }

        protected abstract bool IsBuildIncludedInternal(IBuild build, string input);

        public IEnumerable<string> LocalizedExamples => Examples();

        protected abstract IEnumerable<string> Examples();

        private readonly StringMatcher _stringMatcher = new StringMatcher();

        protected ISearchCriteriaSuggestion AsSuggestion(string suggestion)
        {
            return new SearchCriteriaSuggestion(suggestion);
        }
    }
}