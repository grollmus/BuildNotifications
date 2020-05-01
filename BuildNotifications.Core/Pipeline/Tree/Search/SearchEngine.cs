using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    internal class SearchEngine : ISearchEngine
    {
        public IReadOnlyList<ISearchCriteria> SearchCriterions => _searchCriteria;

        private readonly List<ISearchCriteria> _searchCriteria = new List<ISearchCriteria>();

        private readonly List<ISearchCriteria> _ignoredCriterionsForDefaultSearch = new List<ISearchCriteria>();

        public void AddCriteria(ISearchCriteria criteria, bool includeInDefaultCriteria = true)
        {
            _searchCriteria.Add(criteria);

            if (!includeInDefaultCriteria)
                _ignoredCriterionsForDefaultSearch.Add(criteria);

            _defaultCriteria = new DefaultSearchCriteria(_searchCriteria, _ignoredCriterionsForDefaultSearch);
        }

        private ISearchCriteria _defaultCriteria;

        public const char KeywordSeparator = ':';

        public const char SpecificToGeneralSeparator = ',';

        public SearchEngine()
        {
            _defaultCriteria = new DefaultSearchCriteria(Enumerable.Empty<ISearchCriteria>(), Enumerable.Empty<ISearchCriteria>());
        }

        public ISpecificSearch Parse(string textInput)
        {
            var specificSearch = new SpecificSearch(ParseIntoBlocks(textInput));
            InvokeSearchParsed(textInput, specificSearch);
            return specificSearch;
        }

        public event EventHandler<SearchEngineEventArgs>? SearchParsed;

        private IEnumerable<ISearchBlock> ParseIntoBlocks(string textInput)
        {
            var sb = new StringBuilder();
            var currentCriteria = _defaultCriteria;

            foreach (var character in textInput)
            {
                sb.Append(character);

                if (character == SpecificToGeneralSeparator)
                {
                    var enteredText = sb.ToString();

                    // the separator is not part of the searched text
                    sb.Remove(sb.Length - 1, 1);
                    var searchedTerm = RemoveSpareSpaces(sb.ToString());

                    yield return new SearchBlock(currentCriteria, enteredText, searchedTerm);
                    currentCriteria = _defaultCriteria;
                    sb.Clear();
                    continue;
                }

                if (character != KeywordSeparator)
                    continue;

                var asString = sb.ToString();
                var matchingCriteria = _searchCriteria.FirstOrDefault(c => asString.EndsWith($"{c.LocalizedKeyword}:", StringComparison.OrdinalIgnoreCase));

                if (matchingCriteria == null)
                    continue;

                var keywordLength = $"{matchingCriteria.LocalizedKeyword}:".Length;

                sb.Remove(sb.Length - keywordLength, keywordLength);

                var textUntilKeyword = sb.ToString();
                yield return new SearchBlock(currentCriteria, textUntilKeyword, RemoveSpareSpaces(textUntilKeyword));

                sb.Clear();
                currentCriteria = matchingCriteria;
            }

            var enteredRest = sb.ToString();
            yield return new SearchBlock(currentCriteria, enteredRest, RemoveSpareSpaces(enteredRest));
        }

        private void InvokeSearchParsed(string textInput, ISpecificSearch result) => SearchParsed?.Invoke(this, new SearchEngineEventArgs(result, textInput));

        private string RemoveSpareSpaces(string input) => string.Join(" ", input.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries));
    }
}