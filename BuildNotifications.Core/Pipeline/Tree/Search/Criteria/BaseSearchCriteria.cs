using System;
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
        public int MaxAmountOfSuggestions { get; internal set; } = 5;

        private readonly IPipeline _pipeline;

        private DateTime _lastTimeDataFetchedFromPipeline = DateTime.MinValue;

        protected BaseSearchCriteria(string localizedKeyword, string localizedDescription, IPipeline pipeline)
        {
            _pipeline = pipeline;
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
            UpdateCacheIfNecessary();

            var trimmed = TrimInput(input);
            var isExplicit = trimmed.StartsWith(StringMatcher.ForceExplicitMatchCharacter);
            if (isExplicit)
                trimmed = trimmed.Substring(1);

            _stringMatcher.SearchPattern = trimmed;
            return SuggestInternal(trimmed, _stringMatcher).Take(MaxAmountOfSuggestions).Select(t => isExplicit ? $"={t}" : t).Select(AsSuggestion);
        }

        private void UpdateCacheIfNecessary()
        {
            if (_lastTimeDataFetchedFromPipeline < _pipeline.LastUpdate)
                UpdateCacheForSuggestions(_pipeline);

            _lastTimeDataFetchedFromPipeline = DateTime.Now;
        }

        /// <summary>
        /// Create suggestions for the given input so far.
        /// </summary>
        /// <param name="input">The input entered so far.</param>
        /// <param name="stringMatcher">StringMatcher with the input set. To be used to match suggestions.</param>
        /// <returns>Plain text suggestions for the given input.</returns>
        protected abstract IEnumerable<string> SuggestInternal(string input, StringMatcher stringMatcher);

        /// <summary>
        /// Gets called when the cache for suggestions shall be updated.
        /// </summary>
        /// <param name="pipeline">The pipeline to crawl data from. To be used for suggestions.</param>
        protected abstract void UpdateCacheForSuggestions(IPipeline pipeline);

        public bool IsBuildIncluded(IBuild build, string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return true;

            return IsBuildIncludedInternal(build, TrimInput(input));
        }

        private static string TrimInput(string input) => input.TrimStart(' ').TrimEnd(' ');

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