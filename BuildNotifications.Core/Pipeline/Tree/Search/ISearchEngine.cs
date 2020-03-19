using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    public interface ISearchEngine
    {
        IReadOnlyList<ISearchCriteria> SearchCriterions { get; }

        void AddCriteria(ISearchCriteria criteria);

        ISpecificSearch Parse(string textInput);
    }
}