using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Pipeline.Tree;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Notification
{
    public class BuildNotification : BaseBuildNotification
    {
        public BuildNotification(IList<IBuildNode> buildNodes, BuildStatus status)
            : base(NotificationType.Build, buildNodes, status)
        {
            SetParameters();
        }

        protected override string GetMessageTextId()
        {
            return BuildNodes.Count switch
            {
                1 => BuildChangedTextId,
                _ => BuildsChangedTextId
            };
        }

        protected override string ResolveIssueSource()
        {
            if (BuildNodes.Count == 1)
                return $"{BuildNodes.First().Build.Definition.Name}\n{BuildNodes.First().Build.BranchName}";
            var branchCount = BuildNodes.Select(x => x.Build.BranchName).Distinct().Count();
            var definitionCount = BuildNodes.Select(x => x.Build.Definition.Name).Distinct().Count();
            var branchText = string.Format(StringLocalizer.BranchesCount, branchCount);
            var definitionText = string.Format(StringLocalizer.DefinitionsCount, definitionCount);

            return $"{branchText}\n{definitionText}";
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

        // Build {1} on {2} {0}. E.g. Build Ci on stage failed.
        internal const string BuildChangedTextId = nameof(BuildChangedTextId);

        // {1} builds {0}. E.g. 25 builds failed.
        internal const string BuildsChangedTextId = nameof(BuildsChangedTextId);
    }
}