using System.Collections.Generic;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree
{
    internal class BuildTreeBuildsDelta : IBuildTreeBuildsDelta
    {
        public BuildTreeBuildsDelta(IEnumerable<IBuildNode> currentBuildNodes, Dictionary<(string BuildId, string Project), BuildStatus> previousStatesOfBuildIds)
        {
            // without information of the previous state, a delta is not possible to calculate
            if (previousStatesOfBuildIds == null)
                return;

            foreach (var newBuild in currentBuildNodes)
            {
                if (previousStatesOfBuildIds.TryGetValue((BuildId: newBuild.Build.Id, Project: newBuild.Build.ProjectName), out var previousStatus))
                {
                    if (previousStatus != newBuild.Status)
                        AddToDelta(newBuild);
                }
                else
                    AddToDelta(newBuild);
            }

            void AddToDelta(IBuildNode buildNode)
            {
                switch (buildNode.Status)
                {
                    case BuildStatus.Cancelled:
                        CancelledBuilds.Add(buildNode);
                        break;
                    case BuildStatus.Failed:
                        FailedBuilds.Add(buildNode);
                        break;
                    case BuildStatus.Succeeded:
                    case BuildStatus.PartiallySucceeded:
                        SucceededBuilds.Add(buildNode);
                        break;
                }
            }
        }

        public BuildTreeBuildsDelta()
        {
        }

        public IList<IBuildNode> CancelledBuilds { get; set; } = new List<IBuildNode>();
        public IList<IBuildNode> FailedBuilds { get; set; } = new List<IBuildNode>();

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