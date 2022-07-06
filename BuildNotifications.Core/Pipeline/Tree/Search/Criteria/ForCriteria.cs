using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BuildNotifications.Core.Text;
using BuildNotifications.PluginInterfaces.Builds;

namespace BuildNotifications.Core.Pipeline.Tree.Search.Criteria;

internal class ForCriteria : BaseStringCriteria
{
    public ForCriteria(IPipeline pipeline)
        : base(pipeline)
    {
    }

    public override string LocalizedDescription(CultureInfo forCultureInfo) => StringLocalizer.SearchCriteriaForDescription;

    public override string LocalizedKeyword(CultureInfo forCultureInfo) => StringLocalizer.SearchCriteriaForKeyword;

    protected override IEnumerable<string> Examples()
    {
        yield return StringLocalizer.SearchCriteriaForMeExample;
        yield return string.Join("", StringLocalizer.SearchCriteriaForSomeoneExample.ToLower(CurrentCultureInfo).Take(3));
        yield return "=" + StringLocalizer.SearchCriteriaForSomeoneExample;
        yield return "*" + StringLocalizer.SearchCriteriaForSomeoneExample;
    }

    protected override bool IsBuildIncludedInternal(IBuild build, string input)
    {
        if (build.RequestedFor == null)
            return false;

        if (!StringMatcher.SearchPattern.Equals(input, StringComparison.InvariantCulture))
            StringMatcher.SearchPattern = input;

        return StringMatcher.IsMatch(StringValueOfBuild(build));
    }

    protected override IEnumerable<string> ResolveAllPossibleStringValues(IPipeline pipeline) => pipeline.CachedBuilds().Where(b => b.RequestedFor?.DisplayName != null).Select(b => b.RequestedFor!.DisplayName).Distinct();

    protected override string StringValueOfBuild(IBuild build) => build.RequestedFor?.DisplayName ?? string.Empty;
}