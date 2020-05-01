using System;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    internal sealed class SearchCriteriaSuggestion : ISearchCriteriaSuggestion
    {
        public SearchCriteriaSuggestion(string suggestion)
        {
            Suggestion = suggestion;
        }

        public string Suggestion { get; }

        public bool IsKeyword => false;

        public override bool Equals(object? obj)
        {
            if (obj is ISearchCriteriaSuggestion asSuggestion)
                return Equals(asSuggestion);

            return false;
        }

        private bool Equals(ISearchCriteriaSuggestion other) =>
            other.Suggestion.Equals(Suggestion, StringComparison.InvariantCulture) && other.IsKeyword == IsKeyword;

        public override int GetHashCode() => Suggestion.GetHashCode(StringComparison.InvariantCulture);
    }
}