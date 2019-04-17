using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds;
using BuildNotifications.PluginInterfaces.SourceControl;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class TreeBuilder : ITreeBuilder
    {
        /// <inheritdoc />
        public IBuildTree Build(IEnumerable<IBuild> builds, IEnumerable<IBranch> branches, IEnumerable<IBuildDefinition> definitions)
        {
            return null;
        }
    }
}