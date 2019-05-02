using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class BuildTreeGroupDefinition : IBuildTreeGroupDefinition
    {
        public BuildTreeGroupDefinition()
            : this(Enumerable.Empty<GroupDefinition>())
        {
        }

        public BuildTreeGroupDefinition(IEnumerable<GroupDefinition> definitions)
        {
            _definitions = definitions.ToList();
        }

        public BuildTreeGroupDefinition(params GroupDefinition[] definitions)
            : this(definitions.AsEnumerable())
        {
        }

        /// <inheritdoc />
        public IEnumerator<GroupDefinition> GetEnumerator()
        {
            return _definitions.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _definitions.GetEnumerator();
        }

        private readonly List<GroupDefinition> _definitions;
    }
}