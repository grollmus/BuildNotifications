using System.Collections.Generic;

namespace BuildNotifications.Core.Pipeline.Tree
{
    /// <summary>
    /// Describes which builds updated during the previous version and the now updated BuildTree
    /// </summary>
    public interface IBuildTreeBuildsDelta
    {
        IEnumerable<IBuildNode> Failed { get; }

        IEnumerable<IBuildNode> Cancelled { get; }

        IEnumerable<IBuildNode> Succeeded { get; }

        void RemoveNode(IBuildNode node);

        void Clear();
    }
}