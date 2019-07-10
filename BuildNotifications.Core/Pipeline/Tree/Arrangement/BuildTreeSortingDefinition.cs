using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.Core.Pipeline.Tree.Arrangement
{
    public class BuildTreeSortingDefinition : IBuildTreeSortingDefinition
    {
        public BuildTreeSortingDefinition()
            : this(Enumerable.Empty<SortingDefinition>())
        {
        }

        public BuildTreeSortingDefinition(IEnumerable<SortingDefinition> definitions)
        {
            _definitions = definitions.ToList();
        }

        public BuildTreeSortingDefinition(params SortingDefinition[] definitions)
            : this(definitions.AsEnumerable())
        {
        }

        public IEnumerator<SortingDefinition> GetEnumerator()
        {
            return _definitions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _definitions.GetEnumerator();
        }

        private readonly List<SortingDefinition> _definitions;
    }
}