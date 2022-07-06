using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search;

internal class SpecificSearch : ISpecificSearch
{
    public SpecificSearch(IEnumerable<ISearchBlock> blocks, string searchedTerm)
    {
        SearchedTerm = searchedTerm;
        Blocks = blocks.ToList();
    }

    public override string ToString()
    {
        return $"{{SearchBlocks: \"{string.Join("", Blocks.Select(b => b.ToString()))}\"}}";
    }

    public string SearchedTerm { get; }

    public IReadOnlyList<ISearchBlock> Blocks { get; }

    public IEnumerable<IBuild> ApplySearch(IEnumerable<IBuild> onBuilds) => onBuilds.Where(build => Blocks.All(b => b.IsBuildIncluded(build)));
}