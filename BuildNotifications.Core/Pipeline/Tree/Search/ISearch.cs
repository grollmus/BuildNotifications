using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    public interface ISearch
    {
        IReadOnlyList<ISearchBlock> Blocks { get; }

        IEnumerable<IBuild> ApplySearch(IEnumerable<IBuild> onBuilds);
    }
}