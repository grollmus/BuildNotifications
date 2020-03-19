using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    /// <summary>
    /// Searches through multiple given criteria. If one matches, the build is included.
    /// </summary>
    internal class DefaultSearchCriteria : ISearchCriteria
    {
        private readonly IList<ISearchCriteria> _includedCriterions;

        public DefaultSearchCriteria(IEnumerable<ISearchCriteria> includedCriterions)
        {
            _includedCriterions = includedCriterions.ToList();
        }

        public string LocalizedKeyword => "";

        public IEnumerable<ISearchCriteriaSuggestion> Suggest(string input)
        {
            const int suggestionsToTakeFromEachCriteria = 2;

            foreach (var searchCriteria in _includedCriterions)
            {
                foreach (var suggestion in searchCriteria.Suggest(input).Take(suggestionsToTakeFromEachCriteria))
                {
                    yield return suggestion;
                }
            }
        }

        public bool IsBuildIncluded(IBuild build, string input) => _includedCriterions.Any(c => c.IsBuildIncluded(build, input));
    }
}