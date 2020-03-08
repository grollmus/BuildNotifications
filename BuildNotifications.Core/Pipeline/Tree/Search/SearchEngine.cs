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

        public void AddCriteria(ISearchCriteria criteria)
        {
            _searchCriteria.Add(criteria);
            _defaultCriteria = new DefaultSearchCriteria(_searchCriteria);
        }

        private ISearchCriteria _defaultCriteria;

        private const char KeywordSeparator = ':';

        private const char SpecificToGeneralSeparator = ',';

        public SearchEngine()
        {
            _defaultCriteria = new DefaultSearchCriteria(Enumerable.Empty<ISearchCriteria>());
        }

        public ISpecificSearch Parse(string textInput) => new SpecificSearch(ParseIntoBlocks(textInput));

        private IEnumerable<ISearchBlock> ParseIntoBlocks(string textInput)
        {
            var sb = new StringBuilder();
            var currentCriteria = _defaultCriteria;

            foreach (var character in textInput)
            {
                sb.Append(character);

                if (character == SpecificToGeneralSeparator && currentCriteria != _defaultCriteria)
                {
                    yield return new SearchBlock(currentCriteria, sb.ToString());
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
                if (sb.Length > 0)
                    yield return new SearchBlock(currentCriteria, sb.ToString());

                sb.Clear();
                currentCriteria = matchingCriteria;
            }

            yield return new SearchBlock(currentCriteria, sb.ToString());
        }
    }
}