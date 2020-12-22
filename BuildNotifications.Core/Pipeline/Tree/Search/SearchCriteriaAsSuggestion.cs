using System;
using System.Globalization;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    public sealed class SearchCriteriaAsSuggestion : ISearchCriteriaSuggestion
    {
        public SearchCriteriaAsSuggestion(ISearchCriteria searchCriteria)
        {
            Suggestion = searchCriteria.LocalizedKeyword(CultureInfo.CurrentCulture) + SearchEngine.KeywordSeparator;
        }

        public string Suggestion { get; }

        public bool IsKeyword => true;

        public override bool Equals(object? obj)
        {
            if (obj is ISearchCriteriaSuggestion asSuggestion)
                return Equals(asSuggestion);

            return false;
        }

        private bool Equals(ISearchCriteriaSuggestion other) => other.Suggestion.Equals(Suggestion, StringComparison.InvariantCulture) && other.IsKeyword == IsKeyword;

        public override int GetHashCode() => Suggestion.GetHashCode(StringComparison.InvariantCulture);
    }
}