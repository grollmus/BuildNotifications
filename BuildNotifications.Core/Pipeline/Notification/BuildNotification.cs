using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification
{
    public class BuildNotification : BaseBuildNotification
    {
        // Build {1} on {2} {0}. E.g. Build Ci on stage failed.
        public const string BuildChangedTextId = nameof(BuildChangedTextId);

        // {1} builds {0}. E.g. 25 builds failed.
        public const string BuildsChangedTextId = nameof(BuildsChangedTextId);

        public BuildNotification(IList<IBuildNode> buildNodes, BuildStatus status) : base(NotificationType.Build, buildNodes, status)
        {
            SetParameters();
        }

        private void SetParameters()
        {
            Parameters.Clear();
            var isSingleBuild = BuildNodes.Count == 1;
            Parameters.Add(StatusTextId(isSingleBuild));
            if (isSingleBuild)
            {
                var build = BuildNodes.First();
                Parameters.Add(build.Build.Definition.Name);
                Parameters.Add(build.Build.BranchName);
            }
            else
                Parameters.Add(BuildNodes.Count.ToString());
        }

        protected override string GetMessageTextId() =>
            BuildNodes.Count switch
            {
                1 => BuildChangedTextId,
                _ => BuildsChangedTextId
            };
    }
}