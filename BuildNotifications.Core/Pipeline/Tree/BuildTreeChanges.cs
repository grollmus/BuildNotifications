using System.Collections.Generic;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class BuildTreeBuildsDelta : IBuildTreeBuildsDelta
    {
        public IList<IBuildNode> FailedBuilds { get; set; } = new List<IBuildNode>();

        public IList<IBuildNode> CancelledBuilds { get; set; } = new List<IBuildNode>();

        public IList<IBuildNode> SucceededBuilds { get; set; } = new List<IBuildNode>();

        public IEnumerable<IBuildNode> Failed => FailedBuilds;

        public IEnumerable<IBuildNode> Cancelled => CancelledBuilds;

        public IEnumerable<IBuildNode> Succeeded => SucceededBuilds;

        public void RemoveNode(IBuildNode node)
        {
            FailedBuilds.Remove(node);
            CancelledBuilds.Remove(node);
            SucceededBuilds.Remove(node);
        }

        public void Clear()
        {
            FailedBuilds.Clear();
            SucceededBuilds.Clear();
            CancelledBuilds.Clear();
        }
    }
}