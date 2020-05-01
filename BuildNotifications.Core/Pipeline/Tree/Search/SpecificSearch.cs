using System.Collections.Generic;
using System.Linq;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search
{
    internal class SpecificSearch : ISpecificSearch
    {
        public IReadOnlyList<ISearchBlock> Blocks { get; }

        public SpecificSearch(IEnumerable<ISearchBlock> blocks)
        {
            Blocks = blocks.ToList();
        }

        public IEnumerable<IBuild> ApplySearch(IEnumerable<IBuild> onBuilds) => onBuilds.Where(build => Blocks.All(b => b.IsBuildIncluded(build)));

        public override string ToString()
        {
            return $"{{Specific Search {string.Join("", Blocks.Select(b => b.ToString()))}}}";
        }
    }
}