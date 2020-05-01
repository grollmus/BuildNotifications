using System.Collections.Generic;
using BuildNotifications.Core.Pipeline.Tree.Search;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal interface ITreeBuilder
    {
        IBuildTree Build(IEnumerable<IBuild> builds, IBuildTree? oldTree = null, ISpecificSearch? specificSearch = null);
    }
}