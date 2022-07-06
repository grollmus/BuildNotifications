using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search;

public class EmptySearch : ISpecificSearch
{
    public string SearchedTerm { get; } = string.Empty;

    public IReadOnlyList<ISearchBlock> Blocks { get; } = new List<ISearchBlock>();

    public IEnumerable<IBuild> ApplySearch(IEnumerable<IBuild> onBuilds) => onBuilds;
}