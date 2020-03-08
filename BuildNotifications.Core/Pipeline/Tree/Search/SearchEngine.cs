using System;
using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    internal class SearchEngine : ISearchEngine
    {
        public IReadOnlyList<ISearchCriteria> SearchCriteria => (IReadOnlyList<ISearchCriteria>) _searchCriteria;

        private readonly IList<ISearchCriteria> _searchCriteria = new List<ISearchCriteria>();

        public void AddCriteria(ISearchCriteria criteria) => _searchCriteria.Add(criteria);

        public ISearch Parse(string textInput)
        {
            throw new NotImplementedException();
        }
    }
}
