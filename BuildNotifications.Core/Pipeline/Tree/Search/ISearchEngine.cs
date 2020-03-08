using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    public interface ISearchEngine
    {
        IReadOnlyList<ISearchCriteria> SearchCriteria { get; }

        void AddCriteria(ISearchCriteria criteria);

        ISearch Parse(string textInput);
    }
}