using System;
using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds.Search;

namespace BuildNotifications.Core.Pipeline.Tree.Search;

public interface ISearchEngine
{
    IReadOnlyList<ISearchCriteria> SearchCriterions { get; }

    event EventHandler<SearchEngineEventArgs>? SearchParsed;

    void AddCriteria(ISearchCriteria criteria, bool includeInDefaultCriteria = true);

    ISpecificSearch Parse(string textInput);
}