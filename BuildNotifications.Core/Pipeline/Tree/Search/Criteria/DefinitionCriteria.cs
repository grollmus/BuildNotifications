using System;
using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Config;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
    internal class DefinitionCriteria : BaseSearchCriteria
    {
        public DefinitionCriteria(IPipeline pipeline) : base(StringLocalizer.SearchCriteriaDefinitionKeyword, StringLocalizer.SearchCriteriaDefinitionKeyword, pipeline)
        {
        }

        private readonly StringComparer _stringComparer = StringComparer.FromComparison(StringComparison.InvariantCultureIgnoreCase);

        protected override IEnumerable<string> SuggestInternal(string input, StringMatcher stringMatcher)
        {
            return _validDefinitions.Where(stringMatcher.IsMatch).OrderBy(k => _stringComparer.Compare(input, k));
        }

        private readonly HashSet<string> _validDefinitions = new HashSet<string>();

        protected override void UpdateCacheForSuggestions(IPipeline pipeline)
        {
            _validDefinitions.Clear();
            foreach (var definition in pipeline.CachedDefinitions().Select(b => b.Name).Distinct())
            {
                _validDefinitions.Add(definition);
            }
        }

        private readonly StringMatcher _stringMatcher = new StringMatcher();

        protected override bool IsBuildIncludedInternal(IBuild build, string input)
        {
            if (!_stringMatcher.SearchPattern.Equals(input, StringComparison.InvariantCulture))
                _stringMatcher.SearchPattern = input;

            return _stringMatcher.IsMatch(build.Definition.Name);
        }

        protected override IEnumerable<string> Examples()
        {
            yield return StringLocalizer.SearchCriteriaDefinitionNightlyExample;
            yield return string.Join("", StringLocalizer.SearchCriteriaDefinitionNightlyExample.ToLower(CurrentCultureInfo).Take(3));
            yield return "=" + StringLocalizer.SearchCriteriaDefinitionNightlyExample;
            yield return "*" + StringLocalizer.SearchCriteriaDefinitionNightlyExample;
        }
    }
}