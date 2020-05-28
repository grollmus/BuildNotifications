using System.Collections.Generic;
using System.Linq;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria
{
    internal class BranchCriteria : BaseStringCriteria
    {
        public BranchCriteria(IPipeline pipeline) : base(StringLocalizer.SearchCriteriaBranchKeyword, StringLocalizer.SearchCriteriaBranchDescription, pipeline)
        {
        }

        protected override IEnumerable<string> ResolveAllPossibleStringValues(IPipeline pipeline) => pipeline.CachedBranches().Select(b => b.DisplayName).Distinct();

        protected override string StringValueOfBuild(IBuild build) => build.Branch?.DisplayName ?? string.Empty;

        protected override IEnumerable<string> Examples()
        {
            yield return StringLocalizer.SearchCriteriaBranchStageExample;
            yield return string.Join("", StringLocalizer.SearchCriteriaBranchStageExample.ToLower(CurrentCultureInfo).Take(3));
            yield return "=" + StringLocalizer.SearchCriteriaBranchStageExample;
            yield return "*" + StringLocalizer.SearchCriteriaBranchStageExample;
        }
    }
}